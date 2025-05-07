namespace Card.Application.CQRS.Queries;

public class GetAllDataResult
{
    public GetAllDataResult()
    {
        //intentionally left blank
    }
    public GetAllDataResult(DataEntry[] data)
    {
        Data = data;
    }

    public DataEntry[] Data { get; set; }

    public class DataEntry
    {
        public string Xs { get; set; }
        public string Ys { get; set; }
        public string Id { get; set; }
    }
}