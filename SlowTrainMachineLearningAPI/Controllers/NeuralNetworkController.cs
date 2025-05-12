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
        private readonly IStatelessStateMachine _statelessStateMachine;


        public NeuralNetworkController(ILogger<NeuralNetworkController> logger,
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager requringJobManager,
            INeuralNetworkService neuralNetworkService,
            IStatelessStateMachine statelessStateMachine)
        {
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _requringJobManager = requringJobManager;
            _neuralNetworkService = neuralNetworkService;
            _statelessStateMachine = statelessStateMachine;
            _requringJobManager.AddOrUpdate(
                "TrainModelWithFullData", 
                () => _statelessStateMachine.Build(Guid.NewGuid().ToString(), true), 
                Cron.MinuteInterval(CRON_TRAIN_MODEL_INTERVAL_IN_MINUTES), 
                TimeZoneInfo.Utc);
        }


        [HttpPost("[action]")]
        public async Task<IResult> RebuildNetworkManually(string version)
        {
            // create model and save to Model
            _backgroundJobClient.Enqueue(
                () => _statelessStateMachine.Build(version, false));

            return Results.Ok();
        }

        [HttpPost("[action]")]
        public async Task<IResult> TrainNetwork(RegisterModelRequest commandRequest)
        {
            // create a piece of model and save to Datas
            _backgroundJobClient.Enqueue(
                () => _statelessStateMachine.Train(commandRequest));

            return Results.Ok();
        }

        [HttpPost("[action]")]
        public async Task<IResult> PredictValue(string input)
        {
            return await _statelessStateMachine.Predict(input);
        }
    }
}
