namespace adaptive_deep_learning_model
{
    public interface IStatelessStateMachine
    {
        StatelessStateMachine.State CurrentState { get; }

        void Build();
        void OnFinished();
        void Predict();
        void Train();
    }
}