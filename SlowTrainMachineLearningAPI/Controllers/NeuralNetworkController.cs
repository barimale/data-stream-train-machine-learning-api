using API.SlowTrainMachineLearning.Utilities;
using AutoMapper;
using Card.Application.CQRS.Commands;
using Hangfire;
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
        private readonly PubSub.Hub _hub;

        public NeuralNetworkController(ILogger<NeuralNetworkController> logger,
            IBackgroundJobClient backgroundJobClient,
            IMapper mapper)
        {
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _mapper = mapper;
            _hub = PubSub.Hub.Default;
        }


        [HttpPost("[action]")]
        public async Task<IResult> RebuildNetwork()
        {
            _backgroundJobClient.Enqueue(() => TrainModelWithFullData());

            return Results.Ok();
        }

        [HttpPost("[action]")]
        public async Task<IResult> TrainNetwork(RegisterCardRequest commandRequest)
        {
            var mapped = _mapper.Map<RegisterModelCommand>(commandRequest);
            _hub.Publish(mapped);

            return Results.Ok();
        }

        [HttpPost("[action]")]
        public IResult PredictValue(string input)
        {
            var refToModel = Program.TorchModel;
            var dataBatch = refToModel.Model.TransformInputData(input.ToIntArray());
            var result = refToModel.Model.predict(dataBatch);

            return Results.Ok(JsonSerializer.Serialize(result.data<float>().ToArray()));
        }

        public static async Task TrainModelWithFullData()
        {
            var refToModel = Program.TorchModel;
            await refToModel.LoadFromDB();
            await refToModel.SaveToDB();
        }
    }
}
