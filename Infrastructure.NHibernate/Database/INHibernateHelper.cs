namespace Demo.Infrastructure.Database
{
    public interface INHibernateHelper : IDisposable
    {
         NHibernate.ISession OpenSession();
         NHibernate.IStatelessSession OpenStatelessSesion();
    }
}