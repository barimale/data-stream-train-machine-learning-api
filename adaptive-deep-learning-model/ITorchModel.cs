using adaptive_deep_learning_model;
using Card.Application.CQRS.Queries;

namespace SlowTrainMachineLearningAPI.Model
{
    public interface ITorchModel
    {
        Trivial Model { get; set; }

        Task<CombinedModel> GetModelFromPieces(GetModuleResult mainModel);
        Task LoadFromDB(string version = "latest");
        byte[] ModelToBytes();
        byte[] ModelToBytes(Trivial model);
        Task<bool> SaveToDB(string version = "-");
    }
}