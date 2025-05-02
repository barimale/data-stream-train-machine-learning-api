
using Hangfire;
using Hangfire.MemoryStorage;
using SlowTrainMachineLearningAPI.Model;

namespace SlowTrainMachineLearningAPI
{
    public class Program
    {
        public static TorchModel TorchModel { get; set; } = new TorchModel();
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHangfire(config => config.UseMemoryStorage());
            builder.Services.AddHangfireServer();
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
