using Card.Application.CQRS.Commands;
using SlowTrainMachineLearningAPI;

namespace Albergue.Administrator.HostedServices
{
    public class LocalesHostedService : IHostedService
    {
        private readonly ILogger<LocalesHostedService> _logger;
        private readonly PubSub.Hub _hub;

        public LocalesHostedService()
        {
            _hub = PubSub.Hub.Default;
        }

        public LocalesHostedService(
            ILogger<LocalesHostedService> logger,
            IServiceProvider serviceProvider)
            : this()
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Locales Hosted Service running.");

            _hub.Subscribe<RegisterCardCommand>(async (item) =>
            {
                await DoWorkAsync(item);
            });
        }

        private async Task DoWorkAsync(RegisterCardCommand commandRequest)
        {
            var id = Guid.NewGuid().ToString();

            try
            {
                _logger.LogInformation(
                    "Locales creation in progress. ");

                var refToModel = Program.TorchModel;
                string[] s1 = commandRequest.Input.Trim('[', ']').Split(',');
                int[] myArr = Array.ConvertAll(s1, n => int.Parse(n));

                var dataBatch = refToModel.Model.TransformInputData(myArr);
                //dataBatch to DB
                refToModel.Model.train(dataBatch);

                _logger.LogInformation(
                    "Locales creation is finished. ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Locales Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
