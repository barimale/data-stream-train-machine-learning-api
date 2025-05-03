using Card.Common.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Card.Domain.AggregatesModel.CardAggregate;

public class Card
    : Entity, IAggregateRoot
{
    public DateTime RegisteringTime { get; set; }
    public string AccountNumber { get; set; }
    public string PIN { get; set; }
    public string SerialNumber { get; set; }
    public string Id { get; set; }
    public byte[] Model { get; set; }
    public string Version { get; set; }
}
