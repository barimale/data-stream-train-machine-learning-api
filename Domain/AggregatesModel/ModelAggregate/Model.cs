using Card.Common.Domain;

namespace Domain.AggregatesModel.ModelAggregate;

public class Model
    : Entity, IAggregateRoot
{
    public DateTime RegisteringTime { get; set; }
    public byte[] ModelAsBytes { get; set; }
    public string ModelVersion { get; set; }
}
