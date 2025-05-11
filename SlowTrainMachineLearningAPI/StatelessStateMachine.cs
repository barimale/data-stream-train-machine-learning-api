namespace adaptive_deep_learning_model
{
    public class StatelessStateMachine
    {
        public enum State // Changed from private to public  
        {
            Open,
            InTraining
        }

        private enum Trigger // No change needed here as Trigger is not exposed  
        {
            Train,
            Predict,
            BackToOpen
        }

        private readonly StateMachine<State, Trigger> _machine;

        private readonly Action _dothetraining;
        public StatelessStateMachine(Action dothetraining)
        {
            _machine = new StateMachine<State, Trigger>(State.Open);

            _machine.Configure(State.Open)
                .Permit(Trigger.Train, State.InTraining)
                .Permit(Trigger.Predict, State.Open);

            _machine.Configure(State.InTraining)
                .OnEntry(() => dothetraining.Invoke())
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
