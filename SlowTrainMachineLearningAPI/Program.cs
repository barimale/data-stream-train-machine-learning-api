
using adaptive_deep_learning_model;
using Albergue.Administrator.HostedServices;
using API.SlowTrainMachineLearning.Services;
using Card.API.Extensions;
using Card.API.MappingProfiles;
using Card.Application;
using Card.Infrastructure;
using Hangfire;
using Hangfire.MemoryStorage;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SlowTrainMachineLearningAPI.Model;

namespace SlowTrainMachineLearningAPI
{
    public class Program
    {
        public static TorchModel TorchModel { get; set; }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(typeof(ApiProfile));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHangfire(config => config.UseMemoryStorage());
            builder.Services.AddHangfireServer();
            builder.Services
                   .AddApplicationServices(builder.Configuration)
                   .AddInfrastructureServices(builder.Configuration);

            builder.Services.AddMigration<NNContext>();
            builder.Services.AddHostedService<NeuralNetworkTrainerHostedService>();
            builder.Services.AddTransient<INeuralNetworkService, NeuralNetworkService>();
            builder.Services.AddTransient<IQueueService, QueueService>();
            builder.Services.AddTransient<IQueueConsumerService, QueueConsumerService>();
            builder.Services.AddSingleton<IStatelessStateMachine, StatelessStateMachine>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            var provider = builder.Services.BuildServiceProvider();
            var monitorLoop = app.Services.CreateScope().ServiceProvider.GetRequiredService<ISender>();

            var isender = provider.CreateScope().ServiceProvider.GetRequiredService<ISender>();
            TorchModel = new TorchModel(monitorLoop);

            app.Run();
        }
    }
}
