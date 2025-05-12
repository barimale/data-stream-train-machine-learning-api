using AutoMapper;
using BuildingBlocks.Application.CQRS;
using Card.Application.CQRS.Commands;
using Card.Application.CQRS.Queries;
using Card.Application.Dtos;
using Card.Domain.AggregatesModel.CardAggregate;
using Card.Infrastructure.Repositories;
using MassTransit.Saga;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static Card.Application.CQRS.Queries.GetAllDataResult;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Card.Application.CQRS.QueryHandlers;
public class GetCardHandlers(
    IServiceScopeFactory _serviceScopeFactory,
    IMapper mapper,
    ILogger<GetCardHandlers> logger)
    : IQueryHandler<GetLatestQuery, GetModuleResult>,
    IQueryHandler<TrainNetworkQuery, GetAllDataResult>,
    IQueryHandler<ModelYearsOldInMinutesQuery, GetModelYearsOldResult>,
    IQueryHandler<GetPiecesQuery, GetPiecesResult>
{
    public async Task<GetModuleResult> Handle(GetLatestQuery query, CancellationToken cancellationToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var orderRepository = scopedServices.GetRequiredService<IModelRepository>();

            var card = await orderRepository.GetByLatestAsync(query.Version);
            var mapped = mapper.Map<ModelDto>(card);

            return new GetModuleResult(mapped);
        }

    }

    public async Task<GetAllDataResult> Handle(TrainNetworkQuery request, CancellationToken cancellationToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var dataRepository = scopedServices.GetRequiredService<IDataRepository>();

            var datas = await dataRepository.GetAllUnAppliedAsync();
            var mapped = mapper.Map<List<DataDto>>(datas);

            return new GetAllDataResult(mapped.Select(p => new DataEntry()
            {
                Xs = p.Xs,
                Ys = p.Ys,
                Id = p.Id
            }).ToArray());
        }
    }

    public async Task<GetModelYearsOldResult> Handle(ModelYearsOldInMinutesQuery request, CancellationToken cancellationToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var orderRepository = scopedServices.GetRequiredService<IModelRepository>();

            var inMinutes = await orderRepository.GetYearsOldInMinutesAsync();

            return new GetModelYearsOldResult(inMinutes);
        }
    }

    public async Task<GetPiecesResult> Handle(GetPiecesQuery request, CancellationToken cancellationToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var dataRepository = scopedServices.GetRequiredService<IDataRepository>();

            var card = await dataRepository.GetAllUnAppliedAsync();
            var mapped = mapper.Map<List<DataDto>>(card);

            return new GetPiecesResult(mapped.ToArray());
        }
        
    }
}
