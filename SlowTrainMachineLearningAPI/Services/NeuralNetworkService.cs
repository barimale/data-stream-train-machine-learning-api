using adaptive_deep_learning_model.Utilities;
using AutoMapper;
using Card.Application.CQRS.Commands;
using Card.Application.CQRS.Queries;
using fuzzy_logic_model_generator;
using MediatR;
using SlowTrainMachineLearningAPI;
using System.Text.Json;

namespace API.SlowTrainMachineLearning.Services
{
    public class NeuralNetworkService : INeuralNetworkService
    {
        private readonly ILogger<NeuralNetworkService> _logger;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _provider;
        private readonly IQueueService _queueService;

        public NeuralNetworkService(
            ISender sender,
            IMapper mapper,
            IServiceProvider provider,
            ILogger<NeuralNetworkService> logger,
            IQueueService queueService)
        {
            _provider = provider;
            _mapper = mapper;
            _logger = logger;
            _queueService = queueService;
        }

        
        public async Task DoTrainModelAsync(RegisterModelCommand commandRequest)
        {
            var id = Guid.NewGuid().ToString();

            try
            {
                _logger.LogInformation(
                    "Train neural network in progress. ");

                var refToModel = Program.TorchModel.Model;
                var dataBatch = refToModel
                    .TransformInputData(
                        commandRequest
                        .Xs
                        .ToFloatArray());
                var Ys = refToModel
                    .TransformInputData(
                        commandRequest
                        .Ys
                        .ToFloatArray());

                var loss = refToModel.train(dataBatch, Ys);
                _logger.LogInformation("Loss: {0}", loss);

                using (var scope = _provider.CreateScope())
                {
                    var _sender = scope.ServiceProvider.GetRequiredService<ISender>();
                    var _ = await _sender.Send(new RegisterDataCommand()
                    {
                        Xs = commandRequest.Xs,
                        Ys = commandRequest.Ys,
                        Model = Program.TorchModel.ModelToBytes(refToModel),
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task<IResult> PredictValue(string input)
        {
            // use latest model + combine unapplied pieces
            var transformator = Program.TorchModel.Model;
            GetModuleResult mainModel;
            
            using(var scope = _provider.CreateScope())
            {
                var _sender = scope.ServiceProvider.GetRequiredService<ISender>();
                mainModel = await _sender.Send(new GetLatestQuery(string.Empty));
            }

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

                GetAllDataResult allData;
                using (var scope = _provider.CreateScope())
                {
                    var _sender = scope.ServiceProvider.GetRequiredService<ISender>();
                    allData = await _sender.Send(new TrainNetworkQuery());

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
            var mapped = _mapper.Map<RegisterModelCommand>(commandRequest);
            string msg = JsonSerializer.Serialize(mapped);

            await _queueService.Publish(msg);
        }

        public async Task TrainModelWithFullData(string version)
        {
            var refToModel = Program.TorchModel.Model;

            try
            {
                // fuzzy logic 
                GetModelYearsOldResult modelYearsOldInMinutes;
                GetAllDataResult allData;
                using (var scope = _provider.CreateScope())
                {
                    var _sender = scope.ServiceProvider.GetRequiredService<ISender>();
                    modelYearsOldInMinutes = await _sender.Send(new ModelYearsOldInMinutesQuery());
                    allData = await _sender.Send(new TrainNetworkQuery());


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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
