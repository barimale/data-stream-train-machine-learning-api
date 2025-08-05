using Card.Common.Application.CQRS;

namespace Application.CQRS.Queries;

public class ModelYearsOldInMinutesQuery
    : IQuery<GetModelYearsOldResult>
{
    public ModelYearsOldInMinutesQuery()
    {
    }

}
