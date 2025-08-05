using Application.CQRS.Commands;
using Card.Common.Application.CQRS;
using Domain.AggregatesModel.DataAggregate;
using Microsoft.Extensions.DependencyInjection;

namespace Application.CQRS.CommandHandlers;
public class RegisterDataHandler(IServiceProvider provider)
    : ICommandHandler<RegisterDataCommand, RegisterDataResult>,
    ICommandHandler<UpdateIsAppliedPiece, RegisterDataIsAppliedResult>

{
    public async Task<RegisterDataResult> Handle(RegisterDataCommand command, CancellationToken cancellationToken)
    {
        var card = new Data()
        {
            IngestionTime = DateTime.Now,
            Xs = command.Xs,
            Ys = command.Ys,
            PieceOfModel = command.Model
        };
        using (var scope = provider.CreateAsyncScope())
        {
            var repository = scope.ServiceProvider.GetService<IDataRepository>();

            var result = await repository.AddAsync(card);
            //await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterDataResult(result.Id);
        }
    }

    public async Task<RegisterDataIsAppliedResult> Handle(UpdateIsAppliedPiece request, CancellationToken cancellationToken)
    {
        using (var scope = provider.CreateAsyncScope())
        {
            var dataRepository = scope.ServiceProvider.GetService<IDataRepository>();

            var tobeupdatedId = await dataRepository.SetIsApplied(request.Id);

            //await dataRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterDataIsAppliedResult(tobeupdatedId);
        }
    }
}
