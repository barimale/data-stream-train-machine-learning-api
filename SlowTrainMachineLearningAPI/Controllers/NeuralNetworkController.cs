using Card.Application.CQRS.Commands;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using API.SlowTrainMachineLearning.Services;

namespace API.SlowTrainMachineLearning.Controllers
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
                "BuildModel", 
                () => BuildModel(),
                Cron.MinuteInterval(CRON_TRAIN_MODEL_INTERVAL_IN_MINUTES),
                TimeZoneInfo.Utc);
        }

        [NonAction]
        public void BuildModel()
        {
            _neuralNetworkService.BuildModelWithFullData(Guid.NewGuid().ToString());
        }

        [HttpPost("[action]")]
        public async Task<IResult> RebuildNetworkManually(string version)
        {
            // create model and save to Model
            _backgroundJobClient.Enqueue(
                () => BuildModelManually(version));

            return Results.Ok();
        }

        [NonAction]
        public void BuildModelManually(string version)
        {
            _neuralNetworkService.BuildModelWithFullDataManually(version);
        }

        [HttpPost("[action]")]
        public async Task<IResult> TrainNetwork(RegisterModelRequest commandRequest)
        {
            // create a piece of model and save to Datas
            _backgroundJobClient.Enqueue(
                () => Train(commandRequest));

            return Results.Ok();
        }

        [NonAction]
        public Task Train(RegisterModelRequest commandRequest)
        {
            try
            {
                return _neuralNetworkService.TrainModelOnDemand(commandRequest);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("[action]")]
        public async Task<IResult> PredictValue(string input)
        {
            try
            {
                return await _neuralNetworkService.PredictValue(input);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
