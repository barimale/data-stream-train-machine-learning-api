using RabbitMQ.Client.Events;

namespace API.SlowTrainMachineLearning.Services
{
    public interface IQueueConsumerService
    {
        Task StartAsync(AsyncEventHandler<BasicDeliverEventArgs> body, CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}