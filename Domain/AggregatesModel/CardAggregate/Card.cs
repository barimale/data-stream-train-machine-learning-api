using Card.Common.Domain;

namespace Card.Domain.AggregatesModel.CardAggregate;

public class Card
    : Entity, IAggregateRoot
{
    public DateTime RegisteringTime { get; set; }
    public string AccountNumber { get; set; }
    public string PIN { get; set; }
    public string SerialNumber { get; set; }
    public string Id { get; set; }
}
