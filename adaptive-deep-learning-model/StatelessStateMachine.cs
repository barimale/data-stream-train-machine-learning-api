using API.SlowTrainMachineLearning.Services;
using Card.Application.CQRS.Commands;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlowTrainMachineLearningAPI.Model;
using Stateless;
using System;
using System.Reflection.PortableExecutable;

namespace adaptive_deep_learning_model
{
    public class StatelessStateMachine : IStatelessStateMachine
    {
        public static ITorchModel TorchModel { get; set; }
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<StatelessStateMachine> _logger;
        public enum State { Open, InTraining, InPrediction , InBuilding}

        private enum Trigger { Train, Predict, BackToOpen, Build }

        private readonly StateMachine<State, Trigger> _machine;
        StateMachine<State, Trigger>.TriggerWithParameters<string> _predicateTrigger;
        StateMachine<State, Trigger>.TriggerWithParameters<RegisterModelRequest> _trainTrigger;
        StateMachine<State, Trigger>.TriggerWithParameters<string,bool> _buildTrigger;

        public StatelessStateMachine(
            ILogger<StatelessStateMachine> logger,
            IServiceProvider serviceProvider,
            ITorchModel torchModel)
        {
            TorchModel = torchModel;

            _serviceProvider = serviceProvider;
            _logger = logger;

            _machine = new StateMachine<State, Trigger>(State.Open);

            _predicateTrigger = _machine.SetTriggerParameters<string>(Trigger.Predict);
            _trainTrigger = _machine.SetTriggerParameters<RegisterModelRequest>(Trigger.Train);
            _buildTrigger = _machine.SetTriggerParameters<string,bool>(Trigger.Build);

            _machine.Configure(State.Open)
                .Permit(Trigger.Train, State.InTraining)
                .Permit(Trigger.Build, State.InBuilding)
                .Permit(Trigger.Predict, State.InPrediction);

            _machine.Configure(State.InTraining)
                .OnEntry(() => Console.WriteLine("OnEntry InTraining"))
                .OnEntryFromAsync<RegisterModelRequest>(_trainTrigger,async (volume, t) =>
                {
                    using(var scoped = _serviceProvider.CreateAsyncScope())
                    {
                        var neuralNetworkService = scoped.ServiceProvider.GetService<INeuralNetworkService>();
                        await neuralNetworkService.TrainModelOnDemand(volume);
                        OnTrainingFinished();
                    }
                })
                .Permit(Trigger.BackToOpen, State.Open);

            _machine.Configure(State.InBuilding)
                .OnEntry(() => Console.WriteLine("OnEntry InBuilding"))
                .OnEntryFromAsync<string,bool>(_buildTrigger, async (version,isAutomatic, t) =>
                {
                    using (var scoped = _serviceProvider.CreateAsyncScope())
                    {
                        var neuralNetworkService = scoped.ServiceProvider.GetService<INeuralNetworkService>();
                        await neuralNetworkService.TrainModelWithFullData(version, isAutomatic);
                        OnTrainingFinished();
                    }
                })
                .Permit(Trigger.BackToOpen, State.Open);


            _machine.Configure(State.InPrediction)
                .OnEntry(() => Console.WriteLine("OnEntry InPrediction"))
                //.OnEntryFromAsync<string>(_predicateTrigger, async (volume, t) => await _neuralNetworkService.PredictValue(volume))
                .Permit(Trigger.BackToOpen, State.Open);
        }

        public State CurrentState => _machine.State;

        public void Build(string version, bool isAutomatic)
        {
            _machine.FireAsync(_buildTrigger, version, isAutomatic);
        }
        public void Train(RegisterModelRequest request)
        {
            _machine.FireAsync(_trainTrigger, request);

            //_machine.Fire(Trigger.Train);
        }

        public Task<IResult> Predict(string @value)
        {
            try
            {
                _machine.FireAsync(_predicateTrigger, @value);
                using (var scoped = _serviceProvider.CreateAsyncScope())
                {
                    var neuralNetworkService = scoped.ServiceProvider.GetService<INeuralNetworkService>();
                    return neuralNetworkService.PredictValue(@value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Predict");
            }
            finally
            {
                _machine.FireAsync(Trigger.BackToOpen);
            }

            return null;
        }

        public void OnTrainingFinished()
        {
            _machine.FireAsync(Trigger.BackToOpen);
        }
    }
}
