using Demo.Domain.AggregatesModel.CompanyAggregate;
using Demo.Infrastructure.Database;
using NHibernate;
using NHibernate.Criterion;
using ISession = NHibernate.ISession;

namespace Demo.Infrastructure.Repositories
{

    public class CompanyRepository : ICompanyRepository
    {
        private readonly INHibernateHelper _nHibernateHelper;
        public CompanyRepository(INHibernateHelper nHibernateHelper)
        {
            _nHibernateHelper = nHibernateHelper;
        }

        public async Task<int> Add(Company product)
        {
            using (ISession session = _nHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var result = await session.SaveAsync(product);
                await transaction.CommitAsync();

                return (int)result;
            }
        }
        
        public async Task Update(Company product) 
        {
            using (ISession session = _nHibernateHelper.OpenSession()) 
            using (ITransaction transaction = session.BeginTransaction()) 
            { 
                await session.UpdateAsync(product); 
                await transaction.CommitAsync(); 
            }
        }

        public async Task Remove(Company product) 
        {
            using (ISession session = _nHibernateHelper.OpenSession()) 
            using (ITransaction transaction = session.BeginTransaction()) 
            { 
                await session.DeleteAsync(product); 
                await transaction.CommitAsync(); 
            }
        } 
        
        public async Task<Company> GetById(int productId) 
        {
            using (var session = _nHibernateHelper.OpenStatelessSesion())
            {
                return await session.GetAsync<Company>(productId);
            }
        }
        public async Task<Company> GetBySubquery()
        {
            using (var session = _nHibernateHelper.OpenStatelessSesion())
            {
                var subquery = QueryOver.Of<CompanyAddress>()
                    .Where(c => c.CreationDate > new DateTime(2000, 1, 1))
                    .Select(c => c.Company.Id);

                var query = await session.QueryOver<Company>()
                    .WithSubquery
                    .WhereProperty(c => c.Id)
                    .In(subquery)
                    .SingleOrDefaultAsync();

                return query;
            }
        }


        
    }
}
