using Card.Application.CQRS.Commands;

namespace adaptive_deep_learning_model
{
    public interface IStatelessStateMachine
    {
        StatelessStateMachine.State CurrentState { get; }

        void OnTrainingFinished();
        void Predict(string @value);
        void Train(RegisterModelRequest request);
        void Build(string version);
    }
}