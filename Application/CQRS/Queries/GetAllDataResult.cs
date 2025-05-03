namespace Card.Application.CQRS.Queries;

public class GetAllDataResult
{
    public GetAllDataResult()
    {
        //intentionally left blank
    }
    public GetAllDataResult(string[] data)
    {
        Data = data;
    }

    public string[] Data { get; set; }
}