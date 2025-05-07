using AutoMapper;
using BuildingBlocks.Application.CQRS;
using Card.Application.CQRS.Queries;
using Card.Application.Dtos;
using Card.Domain.AggregatesModel.CardAggregate;
using Microsoft.Extensions.Logging;
using static Card.Application.CQRS.Queries.GetAllDataResult;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Card.Application.CQRS.QueryHandlers;
public class GetCardHandlers(
    IModelRepository orderRepository,
    IDataRepository dataRepository,
    IMapper mapper,
    ILogger<GetCardHandlers> logger)
    : IQueryHandler<GetLatestQuery, GetModuleResult>,
    IQueryHandler<TrainNetworkQuery, GetAllDataResult>,
    IQueryHandler<ModelYearsOldInMinutesQuery, GetModelYearsOldResult>,
    IQueryHandler<GetPiecesQuery, GetPiecesResult>
{
    public async Task<GetModuleResult> Handle(GetLatestQuery query, CancellationToken cancellationToken)
    {
        var card = await orderRepository.GetByLatestAsync(query.Version);
        var mapped = mapper.Map<ModelDto>(card);

        return new GetModuleResult(mapped);
    }

    public async Task<GetAllDataResult> Handle(TrainNetworkQuery request, CancellationToken cancellationToken)
        {
        var datas = await dataRepository.GetAllUnAppliedAsync();
        var mapped = mapper.Map<List<DataDto>>(datas);
        
        return new GetAllDataResult(mapped.Select(p => new DataEntry(){
           Xs = p.Xs,
           Ys = p.Ys,
           Id = p.Id
        }).ToArray());
    }

    public async Task<GetModelYearsOldResult> Handle(ModelYearsOldInMinutesQuery request, CancellationToken cancellationToken)
    {
        var inMinutes = await orderRepository.GetYearsOldInMinutesAsync();

        return new GetModelYearsOldResult(inMinutes);
    }

    public async Task<GetPiecesResult> Handle(GetPiecesQuery request, CancellationToken cancellationToken)
    {
        // WIP 
        var card = await orderRepository.GetByLatestAsync(string.Empty);
        var mapped = mapper.Map<List<ModelDto>>(card);

        // WIP get pieces

        return new GetPiecesResult(mapped.ToArray());
    }
}
