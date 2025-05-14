using BuildingBlocks.Application.CQRS;
using Card.Application.CQRS.Commands;
using Card.Domain.AggregatesModel.CardAggregate;

namespace Card.Application.CQRS.CommandHandlers;
public class RegisterDataHandler(IDataRepository dataRepository)
    : ICommandHandler<RegisterDataCommand, RegisterDataResult>,
    ICommandHandler<UpdateIsAppliedPiece, RegisterDataIsAppliedResult>

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

    public async Task<RegisterDataIsAppliedResult> Handle(UpdateIsAppliedPiece request, CancellationToken cancellationToken)
    {
        var tobeupdatedId = await dataRepository.SetIsApplied(request.Id);

        await dataRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterDataIsAppliedResult(tobeupdatedId);
    }
}
