using BuildingBlocks.Application.CQRS;

namespace Card.Application.CQRS.Queries;

public class GetCardByAccountNumberQuery
    : IQuery<GetCardResult>
{

    public GetCardByAccountNumberQuery(string id)
    {
        Id = id;
    }

    public string Id { get; set; }
}
