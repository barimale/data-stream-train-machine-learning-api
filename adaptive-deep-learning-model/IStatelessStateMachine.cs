using Card.Application.CQRS.Commands;
using Microsoft.AspNetCore.Http;

namespace adaptive_deep_learning_model
{
    public interface IStatelessStateMachine
    {
        StatelessStateMachine.State CurrentState { get; }

        void OnTrainingFinished();
        Task<IResult> Predict(string @value);
        void Train(RegisterModelRequest request);
        void Build(string version);
    }
}