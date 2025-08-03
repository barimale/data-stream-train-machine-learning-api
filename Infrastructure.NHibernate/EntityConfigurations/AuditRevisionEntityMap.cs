using Demo.Domain.AggregatesModel.Company2Aggregate;
using FluentNHibernate.Mapping;

namespace Demo.Infrastructure.EntityConfigurations
{
    public class AuditRevisionEntityMap : ClassMap<AuditRevisionEntity>
    {
        public AuditRevisionEntityMap()
        {
            Table("REVINFO");
            Id(x => x.Id).GeneratedBy.TriggerIdentity();
            Map(x => x.RevisionDate);
        }
    }
}
