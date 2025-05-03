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
    : IQueryHandler<GetCardBySerialNumberQuery, GetCardResult>,
      IQueryHandler<GetCardByIdentifierQuery, GetCardResult>,
      IQueryHandler<GetCardByAccountNumberQuery, GetCardResult>
{
    public async Task<GetCardResult> Handle(GetCardBySerialNumberQuery query, CancellationToken cancellationToken)
    {
        var card = await orderRepository.GetBySerialNumberAsync(query.Id);
        var mapped = mapper.Map<CardDto>(card);

        return new GetCardResult(mapped);
    }

    public async Task<GetCardResult> Handle(GetCardByAccountNumberQuery query, CancellationToken cancellationToken)
    {
        var card = await orderRepository.GetByAccountNumberAsync(query.Id);
        var mapped = mapper.Map<CardDto>(card);

        return new GetCardResult(mapped);
    }

    public async Task<GetCardResult> Handle(GetCardByIdentifierQuery query, CancellationToken cancellationToken)
    {
        var card = await orderRepository.GetByIdAsync(query.Id);
        var mapped = mapper.Map<CardDto>(card);

        return new GetCardResult(mapped);
    }
}
