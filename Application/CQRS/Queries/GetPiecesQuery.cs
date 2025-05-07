using BuildingBlocks.Application.CQRS;

namespace Card.Application.CQRS.Queries;

public class GetPiecesQuery
    : IQuery<GetPiecesResult>
{
    public GetPiecesQuery()
    {
    }
}
