namespace Card.Application.CQRS.Queries;

public class GetModelYearsOldResult
{
    public GetModelYearsOldResult()
    {
        //intentionally left blank
    }
    public GetModelYearsOldResult(double data)
    {
        YearsOldInMinutes = data;
    }

    public double YearsOldInMinutes { get; set; }
}