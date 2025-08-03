using NHibernate.Envers;
using NHibernate.Envers.Configuration.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Domain.AggregatesModel.Company2Aggregate
{
    [RevisionEntity]
    [Table("REVINFO")]
    public class AuditRevisionEntity : DefaultRevisionEntity
    {
        //Intentionally left blank
    }
}
