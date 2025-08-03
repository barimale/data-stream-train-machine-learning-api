using Demo.Domain.AggregatesModel.Company2Aggregate;
using Demo.Infrastructure.Database;
using NHibernate;
using NHibernate.Linq;

namespace Demo.Infrastructure.Repositories
{

    public class Company2Repository : ICompany2Repository
    {
        private readonly INHibernateHelper _nHibernateHelper;
        public Company2Repository(INHibernateHelper nHibernateHelper)
        {
            _nHibernateHelper = nHibernateHelper;
        }

        public async Task<int> Add(Company2 product)
        {
            using (ISession session = _nHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var result = await session.SaveAsync(product);
                await transaction.CommitAsync();

                return (int)result;
            }
        }

        public async Task Update(Company2 product)
        {
            using (ISession session = _nHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                await session.UpdateAsync(product);
                await transaction.CommitAsync();
            }
        }

        public async Task Remove(Company2 product)
        {
            using (ISession session = _nHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                await session.DeleteAsync(product);
                await transaction.CommitAsync();
            }
        }

        public async Task<Company2> GetById(int productId)
        {
            using (var session = _nHibernateHelper.OpenSession())
            {
                return await session.Query<Company2>()
                    .Where( p=> p.Id == productId)
                    .FetchMany(p => p.Addresses)
                    .FirstOrDefaultAsync();
            }
        }
    }
}
