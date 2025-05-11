using Stateless;
using System.Runtime.CompilerServices;

namespace adaptive_deep_learning_model
{
    public class StatelessStateMachine
    {
        //WIP
        //private readonly INeuralNetworkService neuralNetworkService;
        public enum State { Open, InTraining, InPrediction }

        private enum Trigger { Train, Predict, BackToOpen }

        private readonly StateMachine<State, Trigger> _machine;

        public StatelessStateMachine(Action dothetraining, Action dotheprediction)
        {
            _machine = new StateMachine<State, Trigger>(State.Open);

            _machine.Configure(State.Open)
                .Permit(Trigger.Train, State.InTraining)
                .Permit(Trigger.Predict, State.InPrediction);

            _machine.Configure(State.InTraining)
                .OnEntry(() => dothetraining.Invoke())
                .Permit(Trigger.BackToOpen, State.Open);

            _machine.Configure(State.InPrediction)
                .OnEntry(() => dotheprediction.Invoke())
                .Permit(Trigger.BackToOpen, State.Open);
        }

        public State CurrentState => _machine.State;

        public void Train()
        {
            _machine.Fire(Trigger.Train);
        }

        public void Predict()
        {
            _machine.Fire(Trigger.Predict);
        }

        public void OnTrainingFinished()
        {
            _machine.Fire(Trigger.BackToOpen);
        }
    }
}
