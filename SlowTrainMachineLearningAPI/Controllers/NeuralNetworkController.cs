using adaptive_deep_learning_model.Utilities;
using AutoMapper;
using Card.Application.CQRS.Commands;
using Card.Application.CQRS.Queries;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using fuzzy_logic_model_generator;

namespace SlowTrainMachineLearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NeuralNetworkController : ControllerBase
    {
        private readonly ILogger<NeuralNetworkController> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly PubSub.Hub _hub;

        public NeuralNetworkController(ILogger<NeuralNetworkController> logger,
            IBackgroundJobClient backgroundJobClient,
            IMapper mapper,
            ISender sender)
        {
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _mapper = mapper;
            _sender = sender;
            _hub = PubSub.Hub.Default;
        }


        [HttpPost("[action]")]
        public async Task<IResult> RebuildNetwork(string version)
        {
            //WIP it is going to be 60secs interval hosted service job
            //result: trained full model
            _backgroundJobClient.Enqueue(() => TrainModelWithFullData(version));

            return Results.Ok();
        }

        [HttpPost("[action]")]
        public async Task<IResult> TrainNetwork(RegisterModelRequest commandRequest)
        {
            // create piece 
            var mapped = _mapper.Map<RegisterModelCommand>(commandRequest);
            _hub.Publish(mapped);

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
        public async Task TrainModelWithFullData(string version)
        {
            var refToModel = Program.TorchModel.Model;

            try
            {
                var modelYearsOldInMinutes = await _sender.Send(new ModelYearsOldInMinutesQuery());
                var allData = await _sender.Send(new TrainNetworkQuery());
                var pieces = allData.Data.Length;
                var isGenerateModelAllowed = new FuzzyLogicModelGenerator().main(
                    (int)modelYearsOldInMinutes.YearsOldInMinutes, 
                    pieces);

                if (isGenerateModelAllowed)
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
