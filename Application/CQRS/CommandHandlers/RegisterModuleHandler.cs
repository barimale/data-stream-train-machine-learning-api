using Application.CQRS.Commands;
using Card.Common.Application.CQRS;
using Card.Domain.AggregatesModel.CardAggregate;
using Domain.AggregatesModel.ModelAggregate;
using Microsoft.Extensions.DependencyInjection;

namespace Application.CQRS.CommandHandlers;
public class RegisterModuleHandler(IServiceProvider provider)
    : ICommandHandler<RegisterModelCommand, RegisterModelResult>
{
    public async Task<RegisterModelResult> Handle(RegisterModelCommand command, CancellationToken cancellationToken)
    {
        using(var scope = provider.CreateAsyncScope())
        {
            var cardRepository = scope.ServiceProvider.GetService<IModelRepository>();

            var card = new Model()
            {
                RegisteringTime = DateTime.Now,
                ModelAsBytes = command.Model,
                ModelVersion = command.Version,
            };

            var result = await cardRepository.AddAsync(card);
            //await cardRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return new RegisterModelResult(result.Id);
        }        
    }
}
