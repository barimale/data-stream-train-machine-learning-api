using Card.Common.Domain;
using Domain.AggregatesModel.DataAggregate;
using Infrastructure.NHibernate.Database;
using NHibernate;
using NHibernate.Linq;
using ISession = NHibernate.ISession;

namespace Infrastructure.NHibernate.Repositories
{
    public class DataRepository : IDataRepository
    {
        private readonly INHibernateHelper _nHibernateHelper;

        public IUnitOfWork UnitOfWork => throw new NotImplementedException();

        public DataRepository(INHibernateHelper nHibernateHelper)
        {
            _nHibernateHelper = nHibernateHelper;
        }

        public async Task<Data> AddAsync(Data card)
        {
            using (ISession session = _nHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var result = await session.SaveAsync(card);
                await transaction.CommitAsync();

                return (Data)result;
            }
        }

        public async Task<Data> GetByIdAsync(int id)
        {
            using (var session = _nHibernateHelper.OpenStatelessSesion())
            {
                return await session.Query<Data>()
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<IEnumerable<Data>> GetAllAsync()
        {
            using (var session = _nHibernateHelper.OpenStatelessSesion())
            {
                return await session.Query<Data>()
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<Data>> GetAllUnAppliedAsync()
        {
            using (var session = _nHibernateHelper.OpenStatelessSesion())
            {
                return await session.Query<Data>()
                    .Where(p => p.IsApplied == false)
                    .ToListAsync();
            }
        }

        public async Task<int> SetIsApplied(int id)
        {
            using (ISession session = _nHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var product = await session.GetAsync<Data>(id);
                product.IsApplied = true;
                await session.UpdateAsync(product);
                await transaction.CommitAsync();

                return product.Id;
            }
        }
    }
}
