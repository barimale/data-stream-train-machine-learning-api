using Hangfire;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Text.Json;

namespace SlowTrainMachineLearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
        }

        [HttpPost("[action]")]
        public OkResult TrainNetwork()
        {
            // train based on data from DB
            _backgroundJobClient.Enqueue(() => TrainModelWithFullData());

            var refToModel = Program.TorchModel;

            var dataBatch = refToModel.Model.TransformInputData(1, 10, 102);
            //dataBatch to DB
            refToModel.Model.train(dataBatch);

            return Ok();
        }

        [HttpPost("[action]")]
        public OkObjectResult PredictValue()
        {
            var refToModel = Program.TorchModel;

            var dataBatch = refToModel.Model.TransformInputData(1,2,3,4,5,6,7,8,7,6,5,4,3,2,1,2,3,3,333,4,4433,44);

            var result = refToModel.Model.predict(dataBatch);

            return Ok(JsonSerializer.Serialize(result.data<float>().ToArray()));
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
