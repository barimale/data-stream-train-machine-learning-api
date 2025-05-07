using BuildingBlocks.Application.CQRS;
using Card.Application.CQRS.Commands;
using Card.Domain.AggregatesModel.CardAggregate;

namespace Card.Application.CQRS.CommandHandlers;
public class RegisterDataHandler(IDataRepository dataRepository)
    : ICommandHandler<RegisterDataCommand, RegisterDataResult>
{
    public async Task<RegisterDataResult> Handle(RegisterDataCommand command, CancellationToken cancellationToken)
    {
        var card = new Data()
        {
            Id = Guid.NewGuid().ToString(),
            IngestionTime = DateTime.Now,
            Xs = command.Xs,
            Ys = command.Ys,
            PieceOfModel = command.Model
        };

        var result = await dataRepository.AddAsync(card);
        await dataRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterDataResult(result.Id);
    }
}
