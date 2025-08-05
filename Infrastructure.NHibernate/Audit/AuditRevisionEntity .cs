using NHibernate.Envers;
using NHibernate.Envers.Configuration.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.NHibernate.Audit
{
    [RevisionEntity]
    [Table("REVINFO")]
    public class AuditRevisionEntity : DefaultRevisionEntity
    {
        //Intentionally left blank
    }
}
