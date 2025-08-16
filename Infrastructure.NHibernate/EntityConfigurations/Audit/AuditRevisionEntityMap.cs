using FluentNHibernate.Mapping;
using Infrastructure.NHibernate.Audit;

namespace Infrastructure.NHibernate.EntityConfigurations.Audit
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
