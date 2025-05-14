using adaptive_deep_learning_model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Card.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddAdaptiveNetworkServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
