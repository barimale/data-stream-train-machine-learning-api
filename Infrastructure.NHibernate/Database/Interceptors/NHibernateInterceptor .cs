using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.SqlCommand;
using NLog;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace Demo.Infrastructure.Database.Interceptors
{
    public class NHibernateInterceptor : EmptyInterceptor
    {
        private readonly ILogger<NHibernateInterceptor> _logger;

        public NHibernateInterceptor(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger< NHibernateInterceptor >() ?? throw new ArgumentNullException(nameof(logger));
        }

        public override SqlString OnPrepareStatement(SqlString sql)
        {
            _logger.LogDebug($"Executing SQL: {sql}");
            return base.OnPrepareStatement(sql);
        }
    }
}
