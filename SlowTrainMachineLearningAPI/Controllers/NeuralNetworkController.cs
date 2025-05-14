using adaptive_deep_learning_model.Utilities;
using AutoMapper;
using Card.Application.CQRS.Commands;
using Card.Application.CQRS.Queries;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using fuzzy_logic_model_generator;
using RabbitMQ.Client;
using System.Text;
using adaptive_deep_learning_model;
using API.SlowTrainMachineLearning.Services;
using System.Reflection.PortableExecutable;

namespace SlowTrainMachineLearningAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NeuralNetworkController : ControllerBase
    {
        private const int CRON_TRAIN_MODEL_INTERVAL_IN_MINUTES = 20;

        private readonly ILogger<NeuralNetworkController> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _requringJobManager;
        private readonly INeuralNetworkService _neuralNetworkService;
        private readonly IStatelessStateMachine _machine;

        public NeuralNetworkController(ILogger<NeuralNetworkController> logger,
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager requringJobManager,
            IStatelessStateMachine machine,
            INeuralNetworkService neuralNetworkService)
        {
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _requringJobManager = requringJobManager;
            _machine = machine;
            _neuralNetworkService = neuralNetworkService;
            _requringJobManager.AddOrUpdate(
                "TrainModelWithFullData", 
                () => BuildModel(),
                Cron.MinuteInterval(CRON_TRAIN_MODEL_INTERVAL_IN_MINUTES),
                TimeZoneInfo.Utc);
        }

        private void BuildModel()
        {
            _machine.Build();
            _neuralNetworkService.TrainModelWithFullData(Guid.NewGuid().ToString());
            _machine.OnFinished();
        }

        [HttpPost("[action]")]
        public async Task<IResult> RebuildNetworkManually(string version)
        {
            // create model and save to Model
            _backgroundJobClient.Enqueue(
                () => BuildModelManually(version));

            return Results.Ok();
        }

        private void BuildModelManually(string version)
        {
            _machine.Build();
            _neuralNetworkService.TrainModelWithFullDataManually(version);
            _machine.OnFinished();
        }

        [HttpPost("[action]")]
        public async Task<IResult> TrainNetwork(RegisterModelRequest commandRequest)
        {
            // create a piece of model and save to Datas
            _backgroundJobClient.Enqueue(
                () => Train(commandRequest));

            return Results.Ok();
        }

        private Task Train(RegisterModelRequest commandRequest)
        {
            try
            {
                _machine.Train();
                return _neuralNetworkService.TrainModelOnDemand(commandRequest);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                _machine.OnFinished();
            }
        }

        [HttpPost("[action]")]
        public async Task<IResult> PredictValue(string input)
        {
            try
            {
                _machine.Predict();
                return await _neuralNetworkService.PredictValue(input);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                _machine.OnFinished();
            }
        }
    }
}
