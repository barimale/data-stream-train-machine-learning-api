using Demo.Domain.AggregatesModel.Company2Aggregate;
using NHibernate.Envers;

namespace Demo.Infrastructure.Database.Listeners
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
