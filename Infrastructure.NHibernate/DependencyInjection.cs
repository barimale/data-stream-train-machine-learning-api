using Domain.AggregatesModel.DataAggregate;
using Domain.AggregatesModel.ModelAggregate;
using Infrastructure.NHibernate.Database;
using Infrastructure.NHibernate.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.NHibernate;
public static class DependencyInjection
{
    public static IServiceCollection AddNHibernateInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDataRepository, DataRepository>();
        services.AddTransient<INHibernateHelper, NHibernateHelper>();
        services.AddTransient<IModelRepository, ModelRepository>();

        return services;
    }
}
