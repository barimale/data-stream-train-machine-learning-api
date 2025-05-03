using Card.Common.Domain;

namespace Card.Domain.AggregatesModel.CardAggregate
{
    public interface IDataRepository : IRepository<Model>
    {
        Task<Data> AddAsync(Data card);
        Task<string> Delete(string id);
        Task<Data> GetByIdAsync(string id);
        Task<Data> GetByLatestAsync(string id);
    }
}