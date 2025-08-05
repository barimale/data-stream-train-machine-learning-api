using Infrastructure.NHibernate.Audit;
using NHibernate.Envers;

namespace Infrastructure.NHibernate.Database.Listeners
{
    public class AuditRevisionListener : IRevisionListener
    {
        public void NewRevision(object revisionEntity)
        {
            var rev = (AuditRevisionEntity)revisionEntity;
            rev.RevisionDate = DateTime.UtcNow;
        }
    }
}
