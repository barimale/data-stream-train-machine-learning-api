using Card.Application.CQRS.Commands;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using adaptive_deep_learning_model;
using API.SlowTrainMachineLearning.Services;

namespace SlowTrainMachineLearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NeuralNetworkController : ControllerBase
    {
        private const int CRON_TRAIN_MODEL_INTERVAL_IN_MINUTES = 20;

        private readonly ILogger<NeuralNetworkController> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _requringJobManager;
        private readonly INeuralNetworkService _neuralNetworkService;
        private readonly StatelessStateMachine _machine;

        public NeuralNetworkController(ILogger<NeuralNetworkController> logger,
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager requringJobManager,
            INeuralNetworkService neuralNetworkService)
        {
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _requringJobManager = requringJobManager;
            _neuralNetworkService = neuralNetworkService;
            _requringJobManager.AddOrUpdate(
                "TrainModelWithFullData", 
                () => _neuralNetworkService.TrainModelWithFullData(""), 
                Cron.MinuteInterval(CRON_TRAIN_MODEL_INTERVAL_IN_MINUTES), 
                TimeZoneInfo.Utc);

            //    _machine = new StatelessStateMachine(
            //        async () => await TrainModelWithFullData(""),
            //        async () => await PredictValue("0"));
            //    _machine.Train();
            //    _machine.Predict();
        }


        [HttpPost("[action]")]
        public async Task<IResult> RebuildNetworkManually(string version)
        {
            // create model and save to Model
            _backgroundJobClient.Enqueue(
                () => _neuralNetworkService.TrainModelWithFullDataManually(version));

            return Results.Ok();
        }

        [HttpPost("[action]")]
        public async Task<IResult> TrainNetwork(RegisterModelRequest commandRequest)
        {
            // create a piece of model and save to Datas
            _backgroundJobClient.Enqueue(
                () => _neuralNetworkService.TrainModelOnDemand(commandRequest));

            return Results.Ok();
        }

        [HttpPost("[action]")]
        public async Task<IResult> PredictValue(string input)
        {
            return await _neuralNetworkService.PredictValue(input);
        }
    }
}
