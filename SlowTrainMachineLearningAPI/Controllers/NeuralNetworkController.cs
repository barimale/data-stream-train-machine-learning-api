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
            // save mapped to DB via cqrs command
            _hub.Publish(mapped);

            return Results.Ok();
        }

        [HttpPost("[action]")]
        public IResult PredictValue()
        {
            var refToModel = Program.TorchModel;

            var dataBatch = refToModel.Model.TransformInputData(1,2,3,4,5,6,7,8,7,6,5,4,3,2,1,2,3,3,333,4,4433,44);

            var result = refToModel.Model.predict(dataBatch);

            return Results.Ok(JsonSerializer.Serialize(result.data<float>().ToArray()));
        }

        public static async Task TrainModelWithFullData()
        {
            var refToModel = Program.TorchModel;
            refToModel.LoadFromDB();
            // var allData = await _sender.Send(new GetAllCardsQuery());
            //refToModel.Model.train(allData);
            await refToModel.SaveToDB();
        }
    }
}
