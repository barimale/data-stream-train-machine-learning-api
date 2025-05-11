using API.SlowTrainMachineLearning.Services;
using Card.Application.CQRS.Commands;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SlowTrainMachineLearningAPI.Model;
using Stateless;
using System.Reflection.PortableExecutable;

namespace adaptive_deep_learning_model
{
    public class StatelessStateMachine : IStatelessStateMachine
    {
        public static TorchModel TorchModel { get; set; }

        private readonly ISender _sender;

        //WIP
        private readonly INeuralNetworkService _neuralNetworkService;
        private readonly ILogger<StatelessStateMachine> _logger;
        public enum State { Open, InTraining, InPrediction , InBuilding}

        private enum Trigger { Train, Predict, BackToOpen, Build }

        private readonly StateMachine<State, Trigger> _machine;
        StateMachine<State, Trigger>.TriggerWithParameters<string> _predicateTrigger;
        StateMachine<State, Trigger>.TriggerWithParameters<RegisterModelRequest> _trainTrigger;
        StateMachine<State, Trigger>.TriggerWithParameters<string> _buildTrigger;

        public StatelessStateMachine(
            INeuralNetworkService neuralNetworkService,
            ILogger<StatelessStateMachine> logger,
            ISender sender)
        {
            _neuralNetworkService = neuralNetworkService;
            _logger = logger;
            _sender = sender;

            _machine = new StateMachine<State, Trigger>(State.Open);

            _predicateTrigger = _machine.SetTriggerParameters<string>(Trigger.Predict);
            _trainTrigger = _machine.SetTriggerParameters<RegisterModelRequest>(Trigger.Train);
            _buildTrigger = _machine.SetTriggerParameters<string>(Trigger.Build);

            _machine.Configure(State.Open)
                .Permit(Trigger.Train, State.InTraining)
                .Permit(Trigger.Build, State.InBuilding)
                .Permit(Trigger.Predict, State.InPrediction);

            _machine.Configure(State.InTraining)
                .OnEntry(() => Console.WriteLine("OnEntry InTraining"))
                .OnEntryFromAsync<RegisterModelRequest>(_trainTrigger,async (volume, t) => await _neuralNetworkService.TrainModelOnDemand(volume))
                .Permit(Trigger.BackToOpen, State.Open);

            _machine.Configure(State.InBuilding)
                .OnEntry(() => Console.WriteLine("OnEntry InBuilding"))
                .OnEntryFromAsync<string>(_buildTrigger, async (version, t) => await _neuralNetworkService.TrainModelWithFullData(version))
                .Permit(Trigger.BackToOpen, State.Open);


            _machine.Configure(State.InPrediction)
                .OnEntry(() => Console.WriteLine("OnEntry InPrediction"))
                .OnEntryFromAsync<string>(_predicateTrigger, async (volume, t) => await _neuralNetworkService.PredictValue(volume))
                .Permit(Trigger.BackToOpen, State.Open);

            TorchModel = new TorchModel(_sender);
        }

        public State CurrentState => _machine.State;

        public void Build(string version)
        {
            _machine.Fire(_buildTrigger, version);
        }
        public void Train(RegisterModelRequest request)
        {
            _machine.Fire(_trainTrigger, request);

            //_machine.Fire(Trigger.Train);
        }

        public Task<IResult> Predict(string @value)
        {
            try
            {
                _machine.Fire(_predicateTrigger, @value);

                return _neuralNetworkService.PredictValue(@value);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Predict");
            }
            finally
            {
                _machine.Fire(Trigger.BackToOpen);
            }

            return null;
        }

        public void OnTrainingFinished()
        {
            _machine.Fire(Trigger.BackToOpen);
        }
    }
}
