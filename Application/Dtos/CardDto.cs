using TypeGen.Core.TypeAnnotations;

namespace Card.Application.Dtos;

public class CardDto
{
    public CardDto()
    {
        // intentionally left blank
    }

    public DateTime RegisteringTime { get; set; }
    public string AccountNumber { get; set; }
    public string PIN { get; set; }
    public string SerialNumber { get; set; }
    public string Id { get; set; }
}
