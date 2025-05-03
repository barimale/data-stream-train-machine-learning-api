using AutoMapper;
using BuildingBlocks.Application.CQRS;
using Card.Application.CQRS.Queries;
using Card.Application.Dtos;
using Card.Domain.AggregatesModel.CardAggregate;
using Microsoft.Extensions.Logging;

namespace Card.Application.CQRS.QueryHandlers;
public class GetCardHandlers(
    ICardRepository orderRepository,
    IMapper mapper,
    ILogger<GetCardHandlers> logger)
    : IQueryHandler<GetModuleByVersionQuery, GetModuleResult>,
    IQueryHandler<TrainNetworkQuery, GetAllDataResult>
{
    public async Task<GetModuleResult> Handle(GetModuleByVersionQuery query, CancellationToken cancellationToken)
    {
        var card = await orderRepository.GetBySerialNumberAsync(query.Version);
        var mapped = mapper.Map<ModelDto>(card);

        return new GetModuleResult(mapped);
    }

    // WIP
    public async Task<GetAllDataResult> Handle(TrainNetworkQuery request, CancellationToken cancellationToken)
    {
        var card = await orderRepository.GetBySerialNumberAsync(query.Version);
        var mapped = mapper.Map<ModelDto>(card);

        return new GetAllDataResult(["", ""]);
    }
}
