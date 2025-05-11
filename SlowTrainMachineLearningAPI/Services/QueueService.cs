using Card.Application.CQRS.Commands;
using RabbitMQ.Client;
using System.Text;

namespace API.SlowTrainMachineLearning.Services
{
    public class QueueService : IQueueService
    {
        public static string CHANNEL_NAME = "model-creation-channel";
        private readonly ILogger<QueueService> _logger;
        private readonly ConnectionFactory _factory;

        public QueueService(
            ILogger<QueueService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            var hostname = configuration.GetSection("RabbitMQ:HostName").Value;
            _factory = new ConnectionFactory() { HostName = hostname };
        }

        public async Task Publish(string message)
        {
            using var _connection = await _factory.CreateConnectionAsync();
            using var _channel = await _connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(queue: CHANNEL_NAME,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: CHANNEL_NAME,
                Encoding.UTF8.GetBytes(message));
        }

    }
}
