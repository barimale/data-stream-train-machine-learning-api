
namespace API.SlowTrainMachineLearning.Services
{
    public interface IQueueService
    {
        Task Publish(string message);
    }
}