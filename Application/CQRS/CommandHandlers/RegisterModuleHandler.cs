using BuildingBlocks.Application.CQRS;
using Card.Application.CQRS.Commands;
using Card.Application.Integration;
using Card.Domain.AggregatesModel.CardAggregate;

namespace Card.Application.CQRS.CommandHandlers;
public class RegisterModuleHandler(ICardRepository cardRepository, IIdGeneratorAdapter generator)
    : ICommandHandler<RegisterModelCommand, RegisterModelResult>
{
    public async Task<RegisterModelResult> Handle(RegisterModelCommand command, CancellationToken cancellationToken)
    {
        var card = new Model()
        {
            RegisteringTime = DateTime.UtcNow,
            ModelAsBytes = command.Model,
            Version = command.Version,
        };

        var result = await cardRepository.AddAsync(card);
        await cardRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterModelResult(result.Id);
    }
}
