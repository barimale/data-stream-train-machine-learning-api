using BuildingBlocks.Application.CQRS;

namespace Card.Application.CQRS.Queries;

public class TrainNetworkQuery
    : IQuery<GetAllDataResult>
{
    public TrainNetworkQuery()
    {
    }

}
