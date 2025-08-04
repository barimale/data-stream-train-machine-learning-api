using Card.Domain.AggregatesModel.CardAggregate;
using FluentNHibernate.Mapping;

namespace Demo.Infrastructure.EntityConfigurations
{
    public class ModelMap : ClassMap<Model>
    {
        public ModelMap()
        {
            //Table("Product")/*;*/
            Id(u => u.Id).GeneratedBy.TriggerIdentity();
            Map(u => u.RegisteringTime).Nullable();
            Map(u => u.ModelAsBytes).Nullable();
            Map(u => u.ModelVersion).Nullable();
            Version(u => u.Version);
        }
    }
}
