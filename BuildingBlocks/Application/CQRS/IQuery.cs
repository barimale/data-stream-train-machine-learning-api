using MediatR;

namespace BuildingBlocks.Application.CQRS;
public interface IQuery<out TResponse> : IRequest<TResponse>
    where TResponse : notnull
{
}
