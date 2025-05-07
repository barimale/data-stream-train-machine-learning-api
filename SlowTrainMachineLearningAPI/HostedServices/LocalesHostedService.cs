using adaptive_deep_learning_model.Utilities;
using Card.Application.CQRS.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SlowTrainMachineLearningAPI;

namespace Albergue.Administrator.HostedServices
{
    public class LocalesHostedService : IHostedService
    {
        private readonly ILogger<LocalesHostedService> _logger;
        private readonly PubSub.Hub _hub;
        private readonly ISender _sender;
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
            _sender = serviceProvider
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<ISender>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Neural Network Hosted Service running.");

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
                    "Train neural network in progress. ");

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

                var _ = await _sender.Send(new RegisterDataCommand()
                {
                    Xs = commandRequest.Xs,
                    Ys = commandRequest.Ys,
                    Model = refToModel.ModelToBytes(),
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Neural Network Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
