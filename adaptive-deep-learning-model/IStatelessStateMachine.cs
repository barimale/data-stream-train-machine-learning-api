namespace adaptive_deep_learning_model
{
    public interface IStatelessStateMachine
    {
        StatelessStateMachine.State CurrentState { get; }

        void OnTrainingFinished();
        void Predict();
        void Train();
    }
}