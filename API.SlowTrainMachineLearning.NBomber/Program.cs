using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Text;

namespace NBomberTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpClient = Http.CreateDefaultClient();

            var scenario = Scenario.Create("http_scenario", async context =>
            {
                var request =
                    Http.CreateRequest("POST", "https://localhost:44341/NeuralNetwork/TrainNetwork")
                        //.WithHeader("Accept", "text/html");
                    .WithHeader("Accept", "application/json")
                    .WithBody(new StringContent("{ xs: [ 1,2,3,4,5], ys: [ 1,1,1,1,1,1,1,1,1,1] }", Encoding.UTF8, "application/json"));
                // .WithBody(new ByteArrayContent(new [] {1,2,3}))                        

                var response = await Http.Send(httpClient, request);

                return response;
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(
                Simulation.Inject(rate: 100,
                                  interval: TimeSpan.FromSeconds(1),
                                  during: TimeSpan.FromSeconds(5))
            );

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }
    }
}