using Stateless;
using System.Runtime.CompilerServices;

namespace adaptive_deep_learning_model
{
    public class StatelessStateMachine : IStatelessStateMachine
    {
        public enum State { Open, InTraining, InPrediction, InBuilding }

        private enum Trigger { Train, Predict, Build, BackToOpen }

        private readonly StateMachine<State, Trigger> _machine;

        public StatelessStateMachine()
        {
            _machine = new StateMachine<State, Trigger>(State.Open);

            _machine.Configure(State.Open)
                .Permit(Trigger.Train, State.InTraining)
                .Permit(Trigger.Build, State.InBuilding)
                .Permit(Trigger.Predict, State.InPrediction);

            _machine.Configure(State.InBuilding)
                .OnEntry(() => Console.WriteLine("InBuilding"))
                .PermitReentry(Trigger.Predict)
                .Permit(Trigger.BackToOpen, State.Open);

            _machine.Configure(State.InTraining)
                .OnEntry(() => Console.WriteLine("InTraining"))
                .PermitReentry(Trigger.Predict)
                .Permit(Trigger.BackToOpen, State.Open);

            _machine.Configure(State.InPrediction)
                .OnEntry(() => Console.WriteLine("InPrediction"))
                .PermitReentry(Trigger.Predict)
                .Permit(Trigger.BackToOpen, State.Open);
        }

        public State CurrentState => _machine.State;

        public void Train()
        {
            _machine.FireAsync(Trigger.Train);
        }

        public void Predict()
        {
            _machine.FireAsync(Trigger.Predict);
        }

        public void OnFinished()
        {
            _machine.FireAsync(Trigger.BackToOpen);
        }

        public void Build()
        {
            _machine.FireAsync(Trigger.Build);
        }
    }
}
