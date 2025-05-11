using API.SlowTrainMachineLearning.Services;
using Card.Application.CQRS.Commands;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using SlowTrainMachineLearningAPI.Model;
using Stateless;

namespace adaptive_deep_learning_model
{
    public class StatelessStateMachine : IStatelessStateMachine
    {
        public static TorchModel TorchModel { get; set; }

        private readonly ISender _sender;

        //WIP
        private readonly INeuralNetworkService _neuralNetworkService;
        public enum State { Open, InTraining, InPrediction , InBuilding}

        private enum Trigger { Train, Predict, BackToOpen, Build }

        private readonly StateMachine<State, Trigger> _machine;
        StateMachine<State, Trigger>.TriggerWithParameters<string> _predicateTrigger;
        StateMachine<State, Trigger>.TriggerWithParameters<RegisterModelRequest> _trainTrigger;
        StateMachine<State, Trigger>.TriggerWithParameters<string> _buildTrigger;

        public StatelessStateMachine(
            INeuralNetworkService neuralNetworkService,
            ISender sender)
        {
            _neuralNetworkService = neuralNetworkService;
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
                .InternalTransition<RegisterModelRequest>(_trainTrigger,async (volume, t) => await _neuralNetworkService.TrainModelOnDemand(volume))
                .Permit(Trigger.BackToOpen, State.Open);

            _machine.Configure(State.InBuilding)
                .OnEntry(() => Console.WriteLine("OnEntry InBuilding"))
                .InternalTransition<string>(_buildTrigger, async (version, t) => await _neuralNetworkService.TrainModelWithFullData(version))
                .Permit(Trigger.BackToOpen, State.Open);


            _machine.Configure(State.InPrediction)
                .OnEntry(() => Console.WriteLine("OnEntry InPrediction"))
                .InternalTransition<string>(_predicateTrigger, async (volume, t) => await _neuralNetworkService.PredictValue(volume))
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

        public void Predict(string @value)
        {
            _machine.Fire(_predicateTrigger, @value);

            //_machine.Fire(Trigger.Predict);
        }

        public void OnTrainingFinished()
        {
            _machine.Fire(Trigger.BackToOpen);
        }
    }
}
