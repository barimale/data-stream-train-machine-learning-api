using Demo.Domain.AggregatesModel.Company2Aggregate;
using Demo.Infrastructure.Database;
using NHibernate;
using NHibernate.Linq;
using ISession = NHibernate.ISession;

namespace Demo.Infrastructure.Repositories
{
    public class Address2Repository : IAddress2Repository
    {
        private readonly INHibernateHelper _nHibernateHelper;
        public Address2Repository(INHibernateHelper nHibernateHelper)
        {
            _nHibernateHelper = nHibernateHelper;
        }

        public async Task<Company2?> AssignAddressToCompany(int addressId, int companyId)
        {
            using (var session = _nHibernateHelper.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var address = await session.Query<Address2>().Where(p => p.Id == addressId).FetchMany(p => p.Companies).FirstOrDefaultAsync();
                var company = await session.Query<Company2>().Where(p => p.Id == companyId).FetchMany(p => p.Addresses).FirstOrDefaultAsync();

                if (address != null && company != null)
                {
                    address.Companies.Add(company);
                    company.Addresses.Add(address);

                    await session.UpdateAsync(address);
                    await session.UpdateAsync(company);
                    await transaction.CommitAsync();

                    return company;
                }

                return null;
            }
        }

        public async Task<int> Add(Address2 product)
        {
            using (ISession session = _nHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var result = await session.SaveAsync(product);
                await transaction.CommitAsync();

                return (int)result;
            }
        }
        
        public async Task Update(Address2 product) 
        {
            using (ISession session = _nHibernateHelper.OpenSession()) 
            using (ITransaction transaction = session.BeginTransaction()) 
            { 
                await session.UpdateAsync(product); 
                await transaction.CommitAsync(); 
            }
        }

        public async Task Remove(Address2 product) 
        {
            using (ISession session = _nHibernateHelper.OpenSession()) 
            using (ITransaction transaction = session.BeginTransaction()) 
            { 
                await session.DeleteAsync(product); 
                await transaction.CommitAsync(); 
            }
        } 
        
        public async Task<Address2> GetById(int productId) 
        {
            using (var session = _nHibernateHelper.OpenStatelessSesion())
            {
                return await session.Query<Address2>()
                    .Where(p => p.Id == productId)
                    .FetchMany(p => p.Companies)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<Address2> GetByName(string name)
        {
            using (var session = _nHibernateHelper.OpenStatelessSesion())
            {
                // Corrected to use QueryOver for LINQ-like querying
                Address2 product = await session.QueryOver<Address2>()
                    .Where(p => p.Country == name)
                    .Fetch(SelectMode.Fetch, x => x.Companies) // Eagerly fetch related Companies
                    .SingleOrDefaultAsync();

                return product;
            }
        }
    }
}
