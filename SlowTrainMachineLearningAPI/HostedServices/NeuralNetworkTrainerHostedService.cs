using API.SlowTrainMachineLearning.Services;
using Application.CQRS.Commands;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace API.SlowTrainMachineLearning.HostedServices
{
    public class NeuralNetworkTrainerHostedService : IHostedService
    {
        private readonly ILogger<NeuralNetworkTrainerHostedService> _logger;
        private readonly INeuralNetworkService _neuralNetworkService;
        private readonly IQueueConsumerService _queueConsumerService;
        public NeuralNetworkTrainerHostedService(
            ILogger<NeuralNetworkTrainerHostedService> logger,
            INeuralNetworkService neuralNetworkService,
            IQueueConsumerService queueConsumerService,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _neuralNetworkService = neuralNetworkService;
            _queueConsumerService = queueConsumerService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Neural Network Hosted Service running.");

            AsyncEventHandler<BasicDeliverEventArgs> bo = async (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var obj = JsonSerializer.Deserialize<RegisterModelCommand>(body);
                        await _neuralNetworkService.DoTrainModelAsync(obj);
                    };

            await _queueConsumerService.StartAsync(bo, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _queueConsumerService.StopAsync(cancellationToken);
        }
    }
}
