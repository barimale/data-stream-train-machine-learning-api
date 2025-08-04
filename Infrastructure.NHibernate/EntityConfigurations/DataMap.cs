using Card.Domain.AggregatesModel.CardAggregate;
using FluentNHibernate.Mapping;

namespace Demo.Infrastructure.EntityConfigurations
{
    public class DataMap : ClassMap<Data>
    {
        public DataMap()
        {
            //Table("Address")/*;*/
            Id(x => x.Id).GeneratedBy.TriggerIdentity();
            Map(x => x.IngestionTime).Nullable();
            Map(x => x.Xs).Length(250).Nullable();
            Map(x => x.Ys).Length(250).Nullable();
            Map(x => x.PieceOfModel).Nullable();
            Map(x => x.IsApplied).Nullable();
            Version(u => u.Version);
        }
    }
}
