using Demo.Domain.AggregatesModel.Company2Aggregate;
using Demo.Domain.AggregatesModel.CompanyAggregate;
using Demo.Domain.AggregatesModel.ProductAggregate;
using Demo.Infrastructure.Database;
using Demo.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<INHibernateHelper, NHibernateHelper>();
        services.AddTransient<IAddressRepository, AddressRepository>();
        services.AddTransient<IAddress2Repository, Address2Repository>();
        services.AddTransient<ICompanyRepository, CompanyRepository>();
        services.AddTransient<ICompany2Repository, Company2Repository>();

        return services;
    }
}
