using Card.Common.Application.CQRS;

namespace Application.CQRS.Queries;

public class GetPiecesQuery
    : IQuery<GetPiecesResult>
{
    public GetPiecesQuery()
    {
    }
}
