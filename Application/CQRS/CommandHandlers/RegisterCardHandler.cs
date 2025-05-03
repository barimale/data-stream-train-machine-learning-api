using BuildingBlocks.Application.CQRS;
using Card.Application.CQRS.Commands;
using Card.Application.Integration;
using Card.Domain.AggregatesModel.CardAggregate;

namespace Card.Application.CQRS.CommandHandlers;
public class RegisterCardHandler(ICardRepository cardRepository, IIdGeneratorAdapter generator)
    : ICommandHandler<RegisterCardCommand, RegisterCardResult>
{
    private const int ID_LENGTH = 36;

    public async Task<RegisterCardResult> Handle(RegisterCardCommand command, CancellationToken cancellationToken)
    {
        command.Id = generator.Generate(ID_LENGTH);
        var card = new Domain.AggregatesModel.CardAggregate.Card()
        {
            RegisteringTime = DateTime.UtcNow,
            PIN = command.PIN,
            SerialNumber = command.SerialNumber,
            AccountNumber = command.AccountNumber,
            Id = command.Id,
        };

        var result = await cardRepository.AddAsync(card);
        await cardRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterCardResult(result.Id);
    }
}
