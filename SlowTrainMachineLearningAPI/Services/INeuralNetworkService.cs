using Card.Application.CQRS.Commands;

namespace API.SlowTrainMachineLearning.Services
{
    public interface INeuralNetworkService
    {
        Task<IResult> PredictValue(string input);
        Task DoTrainModelAsync(RegisterModelCommand commandRequest);
        Task TrainModelOnDemand(RegisterModelRequest commandRequest);
        Task BuildModelWithFullDataManually(string version);
        Task BuildModelWithFullData(string version);
    }
}