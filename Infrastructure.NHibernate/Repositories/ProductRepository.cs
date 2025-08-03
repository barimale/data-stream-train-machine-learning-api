using Demo.Domain.AggregatesModel.ProductAggregate;
using Demo.Infrastructure.Database;
using NHibernate;
using NHibernate.Linq;
using ISession = NHibernate.ISession;

namespace Demo.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository 
    {
        private readonly INHibernateHelper _nHibernateHelper;
        public ProductRepository(INHibernateHelper nHibernateHelper)
        {
            _nHibernateHelper = nHibernateHelper;
        }

        public async Task<int> Add(Product product)
        {
            using (ISession session = _nHibernateHelper.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var result = await session.SaveAsync(product);
                await transaction.CommitAsync();

                return (int)result;
            }
        }
        
        public async Task Update(Product product) 
        {
            using (ISession session = _nHibernateHelper.OpenSession()) 
            using (ITransaction transaction = session.BeginTransaction()) 
            { 
                await session.UpdateAsync(product); 
                await transaction.CommitAsync(); 
            }
        }

        public async Task Remove(Product product) 
        {
            using (ISession session = _nHibernateHelper.OpenSession()) 
            using (ITransaction transaction = session.BeginTransaction()) 
            { 
                await session.DeleteAsync(product); 
                await transaction.CommitAsync(); 
            }
        } 
        
        public async Task<Product> GetById(int productId) 
        {
            using (var session = _nHibernateHelper.OpenStatelessSesion())
            {
                return await session.GetAsync<Product>(productId);
            }
        }

        public async Task<Product?> GetByName(string name)
        {
            using (var session = _nHibernateHelper.OpenStatelessSesion())
            {
                var product = session.Query<Product>()
                    .Where(p => p.Name == name)
                    .ToFuture();

                return product.FirstOrDefault();
            }
        }
    }
}
