﻿using Card.Domain.AggregatesModel.CardAggregate;
using Card.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Card.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<NNContext>(options =>
        {
            options.UseSqlServer(connectionString,
            sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            });
        }, ServiceLifetime.Transient);

        // Add services to the container.
        services.AddTransient<IModelRepository, ModelRepository>();
        services.AddTransient<IDataRepository, DataRepository>();

        return services;
    }
}
