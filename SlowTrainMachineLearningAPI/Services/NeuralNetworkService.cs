using adaptive_deep_learning_model.Utilities;
using AutoMapper;
using Card.Application.CQRS.Commands;
using Card.Application.CQRS.Queries;
using fuzzy_logic_model_generator;
using MediatR;
using RabbitMQ.Client;
using SlowTrainMachineLearningAPI;
using System.Text;
using System.Text.Json;

namespace API.SlowTrainMachineLearning.Services
{
    public class NeuralNetworkService : INeuralNetworkService
    {
        public static string CHANNEL_NAME = "model-creation-channel";

        private readonly ILogger<NeuralNetworkService> _logger;
        private readonly IMapper _mapper;
        private readonly ISender _sender;

        private readonly ConnectionFactory _factory;


        public NeuralNetworkService(
            ISender sender,
            IMapper mapper,
            ILogger<NeuralNetworkService> logger)
        {
            _sender = sender;
            _mapper = mapper;
            _logger = logger;
            // WIP IConfogiration
            _factory = new ConnectionFactory() { HostName = "localhost" };
        }

        public async Task<IResult> PredictValue(string input)
        {
            // use latest model + combine unapplied pieces
            var transformator = Program.TorchModel.Model;
            var mainModel = await _sender.Send(new GetLatestQuery(string.Empty));

            var refToModel = await Program.TorchModel.GetModelFromPieces(mainModel);
            var dataBatch = transformator.TransformInputData(input.ToFloatArray());
            var result = refToModel.forward(dataBatch);

            return Results.Ok(JsonSerializer.Serialize(result?.data<float>().ToArray()));
        }

        public async Task TrainModelWithFullDataManually(string version)
        {
            var refToModel = Program.TorchModel.Model;

            try
            {
                var allData = await _sender.Send(new TrainNetworkQuery());

                if (allData.Data.Length > 0)
                {
                    await Program.TorchModel.LoadFromDB();

                    foreach (var data in allData.Data)
                    {
                        try
                        {
                            var dataBatch = refToModel.TransformInputData(data.Xs.ToFloatArray());
                            var Ys = refToModel.TransformInputData(data.Ys.ToFloatArray());

                            var loss = refToModel.train(dataBatch, Ys);
                            _logger.LogInformation($"Loss: {loss}");
                            var _ = await _sender.Send(new UpdateIsAppliedPiece(data.Id));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                await Program.TorchModel.SaveToDB(version);
            }
        }

        public async Task TrainModelOnDemand(RegisterModelRequest commandRequest)
        {
            using var _connection = await _factory.CreateConnectionAsync();
            using var _channel = await _connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(queue: CHANNEL_NAME,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

            var mapped = _mapper.Map<RegisterModelCommand>(commandRequest);

            string msg = JsonSerializer.Serialize(mapped);

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: CHANNEL_NAME,
                Encoding.UTF8.GetBytes(msg));
        }

        public async Task TrainModelWithFullData(string version)
        {
            var refToModel = Program.TorchModel.Model;

            try
            {
                // fuzzy logic 
                var modelYearsOldInMinutes = await _sender.Send(new ModelYearsOldInMinutesQuery());
                var allData = await _sender.Send(new TrainNetworkQuery());
                var pieces = allData.Data.Length;
                var isGenerateModelAllowed = new FuzzyLogicModelGenerator().main(
                    (int)modelYearsOldInMinutes.YearsOldInMinutes,
                    pieces);

                _logger.LogInformation("modelYearsOldInMinutes: " + modelYearsOldInMinutes.YearsOldInMinutes);
                _logger.LogInformation("pieces amount: " + pieces);
                _logger.LogInformation("isGenerateModelAllowed: " + isGenerateModelAllowed);

                if (pieces == 0)
                    return;

                if (isGenerateModelAllowed)
                {
                    await Program.TorchModel.LoadFromDB();

                    foreach (var data in allData.Data)
                    {
                        try
                        {
                            var dataBatch = refToModel.TransformInputData(data.Xs.ToFloatArray());
                            var Ys = refToModel.TransformInputData(data.Ys.ToFloatArray());

                            var loss = refToModel.train(dataBatch, Ys);
                            _logger.LogInformation($"Loss: {loss}");
                            var _ = await _sender.Send(new UpdateIsAppliedPiece(data.Id));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                    }

                    await Program.TorchModel.SaveToDB(version);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
