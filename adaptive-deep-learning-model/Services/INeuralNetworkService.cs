using Card.Application.CQRS.Commands;
using Microsoft.AspNetCore.Http;

namespace API.SlowTrainMachineLearning.Services
{
    public interface INeuralNetworkService
    {
        Task<IResult> PredictValue(string input);
        Task DoTrainModelAsync(RegisterModelCommand commandRequest);
        Task TrainModelOnDemand(RegisterModelRequest commandRequest);
        Task TrainModelWithFullData(string version, bool isAutomatic);
    }
}