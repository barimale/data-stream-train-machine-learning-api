using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace adaptive_deep_learning_model;
public static class DependencyInjection
{
    public static IServiceCollection AddAdaptiveNetworkServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
