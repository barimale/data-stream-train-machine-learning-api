using adaptive_deep_learning_model.Utilities;
using AutoMapper;
using Card.Application.CQRS.Commands;
using Card.Application.CQRS.Queries;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using fuzzy_logic_model_generator;
using System;
using Consul;

namespace SlowTrainMachineLearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NeuralNetworkController : ControllerBase
    {
        private const int CRON_TRAIN_MODEL_INTERVAL_IN_MINUTES = 10;

        private readonly ILogger<NeuralNetworkController> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _requringJobManager;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly PubSub.Hub _hub;

        public NeuralNetworkController(ILogger<NeuralNetworkController> logger,
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager requringJobManager,
            IMapper mapper,
            ISender sender)
        {
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _requringJobManager = requringJobManager;
            _mapper = mapper;
            _sender = sender;
            _hub = PubSub.Hub.Default;
            _requringJobManager.AddOrUpdate(
                "TrainModelWithFullData", 
                () => TrainModelWithFullData(""), 
                Cron.MinuteInterval(CRON_TRAIN_MODEL_INTERVAL_IN_MINUTES), 
                TimeZoneInfo.Utc);
        }


        [HttpPost("[action]")]
        public async Task<IResult> RebuildNetworkManually(string version)
        {
            _backgroundJobClient.Enqueue(() => TrainModelWithFullDataManually(version));

            return Results.Ok();
        }

        [HttpPost("[action]")]
        public async Task<IResult> TrainNetwork(RegisterModelRequest commandRequest)
        {
            // create piece 
            _backgroundJobClient.Enqueue(() => TrainModelOnDemand(commandRequest));

            return Results.Ok();
        }

        [HttpPost("[action]")]
        public async Task<IResult> PredictValue(string input)
        {
            // use latest model + combine unapplied pieces
            var transformator = Program.TorchModel.Model;
            var mainModel = await _sender.Send(new GetLatestQuery(string.Empty));

            var refToModel = await Program.TorchModel.GetModelFromPieces(mainModel.Model.ModelAsBytes);
            var dataBatch = transformator.TransformInputData(input.ToFloatArray());
            var result = refToModel.forward(dataBatch);

            return Results.Ok(JsonSerializer.Serialize(result?.data<float>().ToArray()));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task TrainModelOnDemand(RegisterModelRequest commandRequest)
        {
            var mapped = _mapper.Map<RegisterModelCommand>(commandRequest);
            _hub.Publish(mapped);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
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

                if (isGenerateModelAllowed && pieces > 0)
                {
                    await Program.TorchModel.LoadFromDB();

                    foreach (var data in allData.Data)
                    {
                        var dataBatch = refToModel.TransformInputData(data.Xs.ToFloatArray());
                        var Ys = refToModel.TransformInputData(data.Ys.ToFloatArray());

                        var loss = refToModel.train(dataBatch, Ys);
                        _logger.LogInformation($"Loss: {loss}");
                        var _ = await _sender.Send(new UpdateIsAppliedPiece(data.Id));
                    }

                    await Program.TorchModel.SaveToDB(version);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task TrainModelWithFullDataManually(string version)
        {
            var refToModel = Program.TorchModel.Model;

            try
            {
                var modelYearsOldInMinutes = await _sender.Send(new ModelYearsOldInMinutesQuery());
                var allData = await _sender.Send(new TrainNetworkQuery());

                if (allData.Data.Length > 0)
                {
                    await Program.TorchModel.LoadFromDB();

                    foreach (var data in allData.Data)
                    {
                        var dataBatch = refToModel.TransformInputData(data.Xs.ToFloatArray());
                        var Ys = refToModel.TransformInputData(data.Ys.ToFloatArray());

                        var loss = refToModel.train(dataBatch, Ys);
                        _logger.LogInformation($"Loss: {loss}");
                        var _ = await _sender.Send(new UpdateIsAppliedPiece(data.Id));
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
