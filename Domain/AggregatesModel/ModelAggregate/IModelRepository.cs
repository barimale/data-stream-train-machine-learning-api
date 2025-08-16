using Card.Common.Domain;

namespace Domain.AggregatesModel.ModelAggregate
{
    public interface IModelRepository : IRepository<Model>
    {
        Task<Model> AddAsync(Model card);
        Task<Model> GetByLatestAsync(string id);
        Task<IEnumerable<Model>> GetAllAsync();
        Task<double> GetYearsOldInMinutesAsync();

    }
}