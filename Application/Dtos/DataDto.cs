using TypeGen.Core.TypeAnnotations;

namespace Card.Application.Dtos;

public class DataDto
{
    public DataDto()
    {
        // intentionally left blank
    }

    public DateTime IngestionTime { get; set; }
    public string Id { get; set; }
    public string DataAsCommaSeparatedData { get; set; }   
}
