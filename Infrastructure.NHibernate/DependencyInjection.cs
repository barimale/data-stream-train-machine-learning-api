using Card.Domain.AggregatesModel.CardAggregate;
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
        services.AddTransient<IDataRepository, DataRepository>();
        services.AddTransient<INHibernateHelper, NHibernateHelper>();
        services.AddTransient<IModelRepository, ModelRepository>();

        return services;
    }
}
