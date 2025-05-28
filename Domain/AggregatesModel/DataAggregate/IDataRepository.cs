using Card.Common.Domain;

namespace Card.Domain.AggregatesModel.CardAggregate
{
    public interface IDataRepository : IRepository<Data>
    {
        Task<Data> AddAsync(Data card);
        Task<Data> GetByIdAsync(int id);
        Task<IEnumerable<Data>> GetAllAsync();
        Task<IEnumerable<Data>> GetAllUnAppliedAsync();
        Task<int> SetIsApplied(int id);

    }
}