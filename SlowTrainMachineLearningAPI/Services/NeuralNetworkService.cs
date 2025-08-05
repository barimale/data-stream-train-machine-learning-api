using adaptive_deep_learning_model;
using adaptive_deep_learning_model.Utilities;
using Application.CQRS.Commands;
using Application.CQRS.Queries;
using AutoMapper;
using Card.Application.CQRS.Commands;
using fuzzy_logic_model_generator;
using MediatR;
using System.Text.Json;

namespace API.SlowTrainMachineLearning.Services
{
    public class NeuralNetworkService : INeuralNetworkService
    {
        private readonly ILogger<NeuralNetworkService> _logger;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _provider;
        private readonly IQueueService _queueService;
        private readonly ITorchModel _torchModel;
        public NeuralNetworkService(
            ISender sender,
            IMapper mapper,
            IServiceProvider provider,
            ILogger<NeuralNetworkService> logger,
            ITorchModel torchModel,
            IQueueService queueService)
        {
            _provider = provider;
            _mapper = mapper;
            _logger = logger;
            _queueService = queueService;
            _torchModel = torchModel;
        }

        
        public async Task DoTrainModelAsync(RegisterModelCommand commandRequest)
        {
            try
            {
                _logger.LogInformation(
                    "Train neural network in progress. ");

                var refToModel = _torchModel.Model;
                var dataBatch = refToModel
                    .TransformInputData(
                        commandRequest
                        .Xs
                        .ToDoubleArray());
                var Ys = refToModel
                    .TransformInputData(
                        commandRequest
                        .Ys
                        .ToDoubleArray());

                var loss = refToModel.train(dataBatch, Ys);
                _logger.LogInformation("Loss: {0}", loss);

                using (var scope = _provider.CreateScope())
                {
                    var _sender = scope.ServiceProvider.GetRequiredService<ISender>();
                    var _ = await _sender.Send(new RegisterDataCommand()
                    {
                        Xs = commandRequest.Xs,
                        Ys = commandRequest.Ys,
                        Model = _torchModel.ModelToBytes(refToModel),
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
            var transformator = _torchModel.Model;
            GetModuleResult mainModel;
            
            using(var scope = _provider.CreateScope())
            {
                var _sender = scope.ServiceProvider.GetRequiredService<ISender>();
                mainModel = await _sender.Send(new GetLatestQuery(string.Empty));
            }

            var refToModel = await _torchModel.GetModelFromPieces(mainModel);
            var dataBatch = transformator.TransformInputData(input.ToDoubleArray());
            var result = refToModel.forward(dataBatch);

            return Results.Ok(JsonSerializer.Serialize(result?.data<double>().ToArray()));
        }

        public async Task BuildModelWithFullDataManually(string version)
        {
            var refToModel = _torchModel.Model;

            if (_torchModel.IsTrainingInProgress)
                return;

            try
            {
                _torchModel.IsTrainingInProgress = true;
                GetAllDataResult allData;
                using (var scope = _provider.CreateScope())
                {
                    var _sender = scope.ServiceProvider.GetRequiredService<ISender>();
                    allData = await _sender.Send(new TrainNetworkQuery());

                    if (allData.Data.Length > 0)
                    {
                        await _torchModel.LoadFromDB();

                        foreach (var data in allData.Data)
                        {
                            try
                            {
                                var dataBatch = refToModel.TransformInputData(data.Xs.ToDoubleArray());
                                var Ys = refToModel.TransformInputData(data.Ys.ToDoubleArray());

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
                await _torchModel.SaveToDB(version);
                _torchModel.IsTrainingInProgress = false;
            }
        }

        public async Task TrainModelOnDemand(RegisterModelRequest commandRequest)
        {
            var mapped = _mapper.Map<RegisterModelCommand>(commandRequest);
            string msg = JsonSerializer.Serialize(mapped);

            await _queueService.Publish(msg);
        }

        public async Task BuildModelWithFullData(string version)
        {
            var refToModel = _torchModel.Model;

            if (_torchModel.IsTrainingInProgress)
                return;

            try
            {
                _torchModel.IsTrainingInProgress = true;
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
                        await _torchModel.LoadFromDB();

                        foreach (var data in allData.Data)
                        {
                            try
                            {
                                var dataBatch = refToModel.TransformInputData(data.Xs.ToDoubleArray());
                                var Ys = refToModel.TransformInputData(data.Ys.ToDoubleArray());

                                var loss = refToModel.train(dataBatch, Ys);
                                _logger.LogInformation($"Loss: {loss}");
                                var _ = await _sender.Send(new UpdateIsAppliedPiece(data.Id));
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex.Message);
                            }
                        }

                        await _torchModel.SaveToDB(version);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                _torchModel.IsTrainingInProgress = false;
            }
        }
    }
}
