using Card.Application.CQRS.Commands;
using Consul;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;

namespace API.SlowTrainMachineLearning.Services
{
    public class QueueConsumerService : IQueueConsumerService
    {
        private IConnection _connection;
        private IChannel _channel;

        private AsyncEventingBasicConsumer _consumer;
        private ConnectionFactory factory;

        private readonly ILogger<QueueConsumerService> _logger;
        public QueueConsumerService(
            ILogger<QueueConsumerService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            var hostname = configuration.GetSection("RabbitMQ:HostName").Value;
            ushort consumerDispatchConcurrency = ushort.Parse(configuration.GetSection("RabbitMQ:ConsumerDispatchConcurrency").Value);
            factory = new ConnectionFactory() { HostName = hostname, ConsumerDispatchConcurrency = consumerDispatchConcurrency };
        }

        public async Task StartAsync(AsyncEventHandler<BasicDeliverEventArgs> body, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Neural Network Hosted Service running.");

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(queue: QueueService.CHANNEL_NAME,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
            arguments: null);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += body;

            await _channel.BasicConsumeAsync(
                QueueService.CHANNEL_NAME,
                autoAck: true,
                consumer: _consumer);

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Neural Network Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
