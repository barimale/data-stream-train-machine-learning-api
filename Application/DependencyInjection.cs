using BuildingBlocks.Application.Behaviors;
using Card.Application.Behaviours;
using Card.Application.Integration;
using Card.Application.Profiles;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using System.Reflection;

namespace Card.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        services.AddFeatureManagement();
        services.AddAutoMapper(typeof(CardProfile));

        services.AddScoped<IIdGeneratorAdapter, IdGeneratorAdapter>();

        return services;
    }
}
