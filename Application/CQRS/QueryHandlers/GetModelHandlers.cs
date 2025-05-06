using AutoMapper;
using BuildingBlocks.Application.CQRS;
using Card.Application.CQRS.Queries;
using Card.Application.Dtos;
using Card.Domain.AggregatesModel.CardAggregate;
using Microsoft.Extensions.Logging;
using static Card.Application.CQRS.Queries.GetAllDataResult;

namespace Card.Application.CQRS.QueryHandlers;
public class GetCardHandlers(
    IModelRepository orderRepository,
    IDataRepository dataRepository,
    IMapper mapper,
    ILogger<GetCardHandlers> logger)
    : IQueryHandler<GetLatestQuery, GetModuleResult>,
    IQueryHandler<TrainNetworkQuery, GetAllDataResult>,
    IQueryHandler<ModelYearsOldInMinutesQuery, GetModelYearsOldResult>
{
    public async Task<GetModuleResult> Handle(GetLatestQuery query, CancellationToken cancellationToken)
    {
        var card = await orderRepository.GetByLatestAsync(query.Version);
        var mapped = mapper.Map<ModelDto>(card);

        return new GetModuleResult(mapped);
    }

    public async Task<GetAllDataResult> Handle(TrainNetworkQuery request, CancellationToken cancellationToken)
        {
        var datas = await dataRepository.GetAllAsync();
        var mapped = mapper.Map<List<DataDto>>(datas);
        
        return new GetAllDataResult(mapped.Select(p => new DataEntry(){
           Xs = p.Xs,
           Ys = p.Ys
        }).ToArray());
    }

    public async Task<GetModelYearsOldResult> Handle(ModelYearsOldInMinutesQuery request, CancellationToken cancellationToken)
    {
        var inMinutes = await orderRepository.GetYearsOldInMinutesAsync();

        return new GetModelYearsOldResult(inMinutes);
    }
}
