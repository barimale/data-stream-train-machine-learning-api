using adaptive_deep_learning_model;
using API.SlowTrainMachineLearning.Services;
using BuildingBlocks.Application.Behaviors;
using Card.Application.Behaviours;
using Card.Application.Integration;
using Card.Application.Profiles;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using SlowTrainMachineLearningAPI.Model;

namespace Card.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddAdaptiveDeepLearningServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<INeuralNetworkService, NeuralNetworkService>();
        services.AddTransient<IQueueService, QueueService>();
        services.AddTransient<IQueueConsumerService, QueueConsumerService>();
        services.AddTransient<ITorchModel, TorchModel>();
        services.AddSingleton<IStatelessStateMachine, StatelessStateMachine>();

        return services;
    }
}
