using API.SlowTrainMachineLearning.Utilities;
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
            var refToModel = Program.TorchModel.Model;
            var dataBatch = refToModel.TransformInputData(input.ToIntArray());
            var result = refToModel.predict(dataBatch);

            return Results.Ok(JsonSerializer.Serialize(result.data<float>().ToArray()));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public async Task TrainModelWithFullData()
        {
            try
            {
                var refToModel = Program.TorchModel;
                await refToModel.LoadFromDB();
                //WIP
                var allData = await _sender.Send(new TrainNetworkQuery(string.Empty));
                refToModel.Model.train(refToModel.Model.TransformInputData(allData.Data));
                await refToModel.SaveToDB();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
