using adaptive_deep_learning_model.Utilities;
using Card.Application.CQRS.Commands;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SlowTrainMachineLearningAPI;
using SlowTrainMachineLearningAPI.Controllers;
using System.Text.Json;

namespace Albergue.Administrator.HostedServices
{
    public class NeuralNetworkTrainerHostedService : IHostedService
    {
        private readonly ILogger<NeuralNetworkTrainerHostedService> _logger;
        private readonly ISender _sender;
        private IConnection _connection;
        private IChannel _channel;
        private AsyncEventingBasicConsumer _consumer;
        private ConnectionFactory factory;

        public NeuralNetworkTrainerHostedService()
        {
            factory = new ConnectionFactory() { HostName = "localhost", ConsumerDispatchConcurrency = 100 };
        }

        public NeuralNetworkTrainerHostedService(
            ILogger<NeuralNetworkTrainerHostedService> logger,
            IServiceProvider serviceProvider)
            : this()
        {
            _logger = logger; 
            _sender = serviceProvider
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<ISender>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Neural Network Hosted Service running.");

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(queue: NeuralNetworkController.CHANNEL_NAME,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
            arguments: null);

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var obj = JsonSerializer.Deserialize<RegisterModelCommand>(body);
                await DoWorkAsync(obj);
            };

            await _channel.BasicConsumeAsync(
                NeuralNetworkController.CHANNEL_NAME,
                autoAck: true, 
                consumer: _consumer);

        }

        private async Task DoWorkAsync(RegisterModelCommand commandRequest)
        {
            var id = Guid.NewGuid().ToString();

            try
            {
                _logger.LogInformation(
                    "Train neural network in progress. ");

                var refToModel = Program.TorchModel.Model;
                var dataBatch = refToModel
                    .TransformInputData(
                        commandRequest
                        .Xs
                        .ToFloatArray());
                var Ys = refToModel
                    .TransformInputData(
                        commandRequest
                        .Ys
                        .ToFloatArray());
                                
                var loss = refToModel.train(dataBatch, Ys);
                _logger.LogInformation("Loss: {0}", loss);

                var _ = await _sender.Send(new RegisterDataCommand()
                {
                    Xs = commandRequest.Xs,
                    Ys = commandRequest.Ys,
                    Model = Program.TorchModel.ModelToBytes(refToModel),
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Neural Network Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
