using Card.Common.Domain;

namespace Card.Domain.AggregatesModel.CardAggregate
{
    public interface IDataRepository : IRepository<Data>
    {
        Task<Data> AddAsync(Data card);
        Task<string> Delete(string id);
        Task<Data> GetByIdAsync(string id);
        Task<IEnumerable<Data>> GetAllAsync();
    }
}