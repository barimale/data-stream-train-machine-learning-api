using Card.Common.Domain;
using Card.Domain.AggregatesModel.CardAggregate;
using Demo.Infrastructure.Database;
using NHibernate;
using NHibernate.Linq;
using ISession = NHibernate.ISession;

namespace Demo.Infrastructure.Repositories
{
   public class ModelRepository : IModelRepository
    {
        private readonly INHibernateHelper _nHibernateHelper;

        public IUnitOfWork UnitOfWork => throw new NotImplementedException();

        public ModelRepository(INHibernateHelper nHibernateHelper)
        {
            _nHibernateHelper = nHibernateHelper;
        }

        public async Task<Model> AddAsync(Model card)
        {
            using (ISession session = _nHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var result = await session.SaveAsync(card);
                await transaction.CommitAsync();

                return (Model)result;
            }
        }

        public async Task<Model> GetByLatestAsync(string id)
        {
            using (var session = _nHibernateHelper.OpenStatelessSesion())
            {
                return await session.Query<Model>()
                    .Where(p => p.ModelVersion == "latest")
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<IEnumerable<Model>> GetAllAsync()
        {
            using (var session = _nHibernateHelper.OpenStatelessSesion())
            {
                return await session.Query<Model>()
                    .OrderBy(p => p.RegisteringTime)
                    .ToListAsync();
            }
        }

        public async Task<double> GetYearsOldInMinutesAsync()
        {
            var result = await GetByLatestAsync(string.Empty);
            if (result is null)
            {
                return 0;
            }

            DateTimeOffset from = new DateTimeOffset(result.RegisteringTime);
            DateTimeOffset now = DateTime.Now;

            return (now - from).TotalMinutes;
        }
    }
}
