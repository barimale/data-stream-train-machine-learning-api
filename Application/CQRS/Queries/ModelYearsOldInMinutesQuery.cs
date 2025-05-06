using BuildingBlocks.Application.CQRS;

namespace Card.Application.CQRS.Queries;

public class ModelYearsOldInMinutesQuery
    : IQuery<GetModelYearsOldResult>
{
    public ModelYearsOldInMinutesQuery()
    {
    }

}
