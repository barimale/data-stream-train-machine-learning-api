using Card.Common.Domain;

namespace Card.Domain.AggregatesModel.CardAggregate;

public class Data
    : Entity, IAggregateRoot
{
    public DateTime IngestionTime { get; set; }
    public string Id { get; set; }
    public string DataAsCommaSeparatedData { get; set; }
    public string Ys { get; set; }
}
