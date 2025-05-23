using BuildingBlocks.Application.CQRS;
using Card.Application.CQRS.Commands;
using Card.Common.Domain;
using Card.Domain.AggregatesModel.CardAggregate;
using Card.Infrastructure.Repositories;
using Consul;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Card.Application.CQRS.CommandHandlers;
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
            await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterDataResult(result.Id);
        }
    }

    public async Task<RegisterDataIsAppliedResult> Handle(UpdateIsAppliedPiece request, CancellationToken cancellationToken)
    {
        using (var scope = provider.CreateAsyncScope())
        {
            var dataRepository = scope.ServiceProvider.GetService<IDataRepository>();

            var tobeupdatedId = await dataRepository.SetIsApplied(request.Id);

            await dataRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterDataIsAppliedResult(tobeupdatedId);
        }
    }
}
