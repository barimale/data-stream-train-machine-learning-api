using Application.CQRS.Queries;

namespace adaptive_deep_learning_model
{
    public interface ITorchModel
    {
        Trivial Model { get; }
        Task<CombinedModel> GetModelFromPieces(GetModuleResult mainModel);
        Task LoadFromDB(string version = "latest");
        byte[] ModelToBytes();
        byte[] ModelToBytes(Trivial model);
        Task<bool> SaveToDB(string version = "-");
        bool IsTrainingInProgress { get; set; }
    }
}