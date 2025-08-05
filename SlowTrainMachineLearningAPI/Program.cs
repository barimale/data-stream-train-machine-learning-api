using API.SlowTrainMachineLearning.HostedServices;
using API.SlowTrainMachineLearning.Services;
using Card.API.Extensions;
using Card.API.MappingProfiles;
using Card.Application;
using Card.Infrastructure;
using Hangfire;
using Hangfire.MemoryStorage;
using SlowTrainMachineLearningAPI.Model;
using Demo.Infrastructure;

namespace SlowTrainMachineLearningAPI
{
    public class Program
    {
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
                   .AddAdaptiveNetworkServices(builder.Configuration)
                   .AddApplicationServices(builder.Configuration)
                   .AddNHibernateInfrastructureServices(builder.Configuration);

            builder.Services.AddMigration<NNContext>();
            builder.Services.AddHostedService<NeuralNetworkTrainerHostedService>();
            builder.Services.AddTransient<INeuralNetworkService, NeuralNetworkService>();
            builder.Services.AddTransient<IQueueService, QueueService>();
            builder.Services.AddTransient<IQueueConsumerService, QueueConsumerService>();
            // just one model in app
            builder.Services.AddSingleton<ITorchModel, TorchModel>();

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

            app.Run();
        }
    }
}
