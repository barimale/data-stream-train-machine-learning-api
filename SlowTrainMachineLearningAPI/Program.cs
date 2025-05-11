
using adaptive_deep_learning_model;
using Albergue.Administrator.HostedServices;
using Card.API.Extensions;
using Card.API.MappingProfiles;
using Card.Application;
using Card.Infrastructure;
using Hangfire;
using Hangfire.MemoryStorage;

namespace SlowTrainMachineLearningAPI
{
    public class Program
    {
        //public static IStatelessStateMachine statelessStateMachine;
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<IStatelessStateMachine, StatelessStateMachine>();

            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(typeof(ApiProfile));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHangfire(config => config.UseMemoryStorage());
            builder.Services.AddHangfireServer();
            builder.Services
                   .AddApplicationServices(builder.Configuration)
                   .AddAdaptiveDeepLearningServices(builder.Configuration)
                   .AddInfrastructureServices(builder.Configuration);

            builder.Services.AddMigration<NNContext>();
            builder.Services.AddHostedService<NeuralNetworkTrainerHostedService>();
           
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

            //var serviceProvider = builder.Services.BuildServiceProvider();

            //// Step 3: Retrieve the service
            //var myService = serviceProvider.GetService<IStatelessStateMachine>();

            //statelessStateMachine = myService;
            app.Run();
        }
    }
}
