using Hangfire;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SlowTrainMachineLearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _backgroundJobClient.Enqueue(() => LongRunningOperation());

            var refToModel = Program.TorchModel;

            var dataBatch = refToModel.Model.TransformInputData();

            refToModel.Model.train(dataBatch);

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost(Name = "Postdummy")]
        public OkObjectResult Post()
        {
            var refToModel = Program.TorchModel;

            var dataBatch = refToModel.Model.TransformInputData();

            var result = refToModel.Model.test(dataBatch);

            return Ok(JsonSerializer.Serialize(result));
        }

        public static async Task LongRunningOperation()
        {
            // Simulate long-running task
            var refToModel = Program.TorchModel;
            refToModel.LoadFromDB();
            await refToModel.SaveToDB();
        }
    }
}
