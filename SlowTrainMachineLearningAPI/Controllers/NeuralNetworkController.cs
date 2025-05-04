using adaptive_deep_learning_model.Utilities;
using AutoMapper;
using Card.Application.CQRS.Commands;
using Card.Application.CQRS.Queries;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
            _backgroundJobClient.Enqueue(() => TrainModelWithFullData(version));

            return Results.Ok();
        }

        [HttpPost("[action]")]
        public async Task<IResult> TrainNetwork(RegisterCardRequest commandRequest)
        {
            var _ = _sender.Send(new RegisterDataCommand() { 
                Input = commandRequest.Input,
                Ys = commandRequest.Ys});

            var mapped = _mapper.Map<RegisterModelCommand>(commandRequest);
            _hub.Publish(mapped);

            return Results.Ok();
        }

        [HttpPost("[action]")]
        public IResult PredictValue(string input)
        {
            var refToModel = Program.TorchModel.Model;
            var dataBatch = refToModel.TransformInputData(input.ToFloatArray());
            var result = refToModel.predict(dataBatch);

            return Results.Ok(JsonSerializer.Serialize(result.data<float>().ToArray()));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task TrainModelWithFullData(string version)
        {
            var refToModel = Program.TorchModel.Model;

            try
            {
                var allData = await _sender.Send(new TrainNetworkQuery());
                if (allData.Data.Length > 0)
                {
                    foreach(var data in allData.Data)
                    {
                        var dataBatch = refToModel.TransformInputData(data.DataX.ToFloatArray());
                        var Ys = refToModel.TransformInputData(data.Y.ToFloatArray());

                        var loss = refToModel.train(dataBatch, Ys);
                        _logger.LogInformation($"Loss: {loss}");
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
    }
}
