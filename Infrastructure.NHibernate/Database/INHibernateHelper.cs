using NHibernate;

namespace Infrastructure.NHibernate.Database
{
    public interface INHibernateHelper : IDisposable
    {
         ISession OpenSession();
         IStatelessSession OpenStatelessSesion();
    }
}