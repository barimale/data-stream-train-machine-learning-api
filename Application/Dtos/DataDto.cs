using TypeGen.Core.TypeAnnotations;

namespace Card.Application.Dtos;

public class DataDto
{
    public DataDto()
    {
        // intentionally left blank
    }

    public DateTime IngestionTime { get; set; }
    public int Id { get; set; }
    public string Xs { get; set; }   
    public string Ys { get; set; }
    public byte[] PieceOfModel { get; set; }
}
