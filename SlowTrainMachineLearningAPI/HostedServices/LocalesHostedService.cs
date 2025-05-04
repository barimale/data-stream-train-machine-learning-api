using adaptive_deep_learning_model.Utilities;
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

            _hub.Subscribe<RegisterModelCommand>(async (item) =>
            {
                await DoWorkAsync(item);
            });
        }

        private async Task DoWorkAsync(RegisterModelCommand commandRequest)
        {
            var id = Guid.NewGuid().ToString();

            try
            {
                _logger.LogInformation(
                    "Locales creation in progress. ");

                var refToModel = Program.TorchModel;
                var dataBatch = refToModel
                    .Model
                    .TransformInputData(
                        commandRequest
                        .Xs
                        .ToFloatArray());
                var Ys = refToModel
                    .Model
                    .TransformInputData(
                        commandRequest
                        .Ys
                        .ToFloatArray());
                                
                var loss = refToModel.Model.train(dataBatch, Ys);
                _logger.LogInformation("Loss: {0}", loss);

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
