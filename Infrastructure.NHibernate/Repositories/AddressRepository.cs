using Demo.Domain.AggregatesModel.CompanyAggregate;
using Demo.Infrastructure.Database;
using NHibernate;
using NHibernate.Linq;
using ISession = NHibernate.ISession;

namespace Demo.Infrastructure.Repositories
{
   public class AddressRepository : IAddressRepository
    {
        private readonly INHibernateHelper _nHibernateHelper;
        public AddressRepository(INHibernateHelper nHibernateHelper)
        {
            _nHibernateHelper = nHibernateHelper;
        }

        public object AssingAddressToCompany(int addressId, int companyId, string description)
        {
            using (var session = _nHibernateHelper.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var address = session.Get<Address>(addressId);
                var company = session.Get<Company>(companyId);

                if (address != null && company != null)
                {
                    address.Companies.Add(company);
                    company.Addresses.Add(address);

                    session.Update(address);
                    session.Update(company);

                    var caid = session.Save(new CompanyAddress()
                    {
                        Description = description,
                        Company = company,
                        Address = address,
                        CreationDate = DateTime.Now,
                    });

                    transaction.Commit();

                    return caid;
                }

                return null;
            }
        }

        public async Task<int> Add(Address product)
        {
            using (ISession session = _nHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var result = await session.SaveAsync(product);
                await transaction.CommitAsync();

                return (int)result;
            }
        }
        
        public async Task Update(Address product) 
        {
            using (ISession session = _nHibernateHelper.OpenSession()) 
            using (ITransaction transaction = session.BeginTransaction()) 
            { 
                await session.UpdateAsync(product); 
                await transaction.CommitAsync(); 
            }
        }

        public async Task Remove(Address product) 
        {
            using (ISession session = _nHibernateHelper.OpenSession()) 
            using (ITransaction transaction = session.BeginTransaction()) 
            { 
                await session.DeleteAsync(product); 
                await transaction.CommitAsync(); 
            }
        } 
        
        public async Task<Address> GetById(int productId) 
        {
            using (var session = _nHibernateHelper.OpenStatelessSesion())
            {
                return await session.Query<Address>()
                    .Where(p => p.Id == productId)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<Address> GetByCountry(string country)
        {
            using (var session = _nHibernateHelper.OpenStatelessSesion())
            {
                // Corrected to use QueryOver for LINQ-like querying
                Address product = await session.Query<Address>()
                    .Where(p => p.Country == country)
                    //.Fetch(SelectMode.JoinOnly, x => x.Companies) // Eagerly fetch related Companies
                    .FirstOrDefaultAsync();

                return product;
            }
        }
    }
}
