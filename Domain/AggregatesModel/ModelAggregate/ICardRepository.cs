using Card.Common.Domain;

namespace Card.Domain.AggregatesModel.CardAggregate
{
    public interface ICardRepository : IRepository<Model>
    {
        Task<Model> AddAsync(Model card);
        Task<string> Delete(string id);
        Task<Model> GetByIdAsync(string id);
        Task<Model> GetByLatestAsync(string id);
    }
}