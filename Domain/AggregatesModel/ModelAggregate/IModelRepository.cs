using Card.Common.Domain;

namespace Card.Domain.AggregatesModel.CardAggregate
{
    public interface IModelRepository : IRepository<Model>
    {
        Task<Model> AddAsync(Model card);
        Task<Model> GetByLatestAsync(string id);
    }
}