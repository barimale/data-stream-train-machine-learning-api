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
        public string DataX { get; set; }
        public string Y { get; set; }
    }
}