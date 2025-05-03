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
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public NeuralNetworkController(ILogger<NeuralNetworkController> logger,
            IBackgroundJobClient backgroundJobClient,
            ISender sender,
            IMapper mapper)
        {
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _sender = sender;
            _mapper = mapper;
        }

        [HttpPost("[action]")]
        public async Task<IResult> TrainNetwork(string input)
        {
            // input transform to tensor then train and return loss
            var mapped = _mapper.Map<RegisterCardCommand>(input);
            var response = await _sender.Send(mapped);

            //if (response is null)
            //    return Results.NotFound();

            //return Results.Ok(response);
            // train based on data from DB
            _backgroundJobClient.Enqueue(() => TrainModelWithFullData());

            var refToModel = Program.TorchModel;
            string[] s1 = input.Trim('[', ']').Split(',');
            int[] myArr = Array.ConvertAll(s1, n => int.Parse(n));

            var dataBatch = refToModel.Model.TransformInputData(myArr);
            //dataBatch to DB
            refToModel.Model.train(dataBatch);

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
            // Simulate long-running task
            var refToModel = Program.TorchModel;
            refToModel.LoadFromDB();
            await refToModel.SaveToDB();
        }
    }
}
