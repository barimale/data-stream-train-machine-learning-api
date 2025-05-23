using Card.Common.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Card.Domain.AggregatesModel.CardAggregate;

public class Model
    : Entity, IAggregateRoot
{
    public DateTime RegisteringTime { get; set; }
    public byte[] ModelAsBytes { get; set; }
    public string Version { get; set; }
}
