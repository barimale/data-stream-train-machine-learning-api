using Demo.Domain.AggregatesModel.Company2Aggregate;
using Demo.Domain.AggregatesModel.ProductAggregate;
using Demo.Infrastructure.Database;
using Demo.Infrastructure.Database.Interceptors;
using Demo.Infrastructure.Database.Listeners;
using Demo.Infrastructure.EntityConfigurations;
using Demo.Migrations.Conventions;
using Demo.Migrations.Migrations;
using FluentMigrator.Runner;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Cfg;
using ISession = NHibernate.ISession;

namespace Demo.Infrastructure
{
    public class NHibernateHelper : INHibernateHelper
    {
        private static readonly object _lock = new();
        private ISessionFactory? _sessionFactory;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private bool _disposed;

        public NHibernateHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("OracleDB")
                ?? throw new InvalidOperationException("Connection string 'OracleDB' not found.");
        }

        private ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    lock (_lock)
                    {
                        if (_sessionFactory == null)
                        {
                            _sessionFactory = BuildSessionFactory();
                        }
                    }
                }
                return _sessionFactory;
            }
        }

        private ISessionFactory BuildSessionFactory()
        {
            var fluentConfig = Fluently.Configure()
                .Database(OracleManagedDataClientConfiguration.Oracle10
                    .ConnectionString(_connectionString)
                    .Driver<NHibernate.Driver.OracleManagedDataClientDriver>())
                .Mappings(m =>
                {
                    m.FluentMappings.AddFromAssemblyOf<ProductTypeMap>().Conventions.Add<LowercaseTableNameConvention>();
                });

#if DEBUG
            fluentConfig.ExposeConfiguration(cfg =>
            {
                var enversConf = new NHibernate.Envers.Configuration.Fluent.FluentConfiguration();
                enversConf.Audit<Company2>();
                enversConf.Audit<Address2>();
                enversConf.Audit<CompanyAddress2>();
                enversConf.Audit<Product>();

                enversConf.SetRevisionEntity<AuditRevisionEntity>(
                    x => x.Id,
                    x => x.RevisionDate, new AuditRevisionListener());
                cfg.IntegrateWithEnvers(enversConf);

                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Debug);
                    builder.AddConsole();
                });

                cfg.SetInterceptor(new NHibernateInterceptor(loggerFactory));

                var serviceProvider = CreateServices(_connectionString);

                using (var scope = serviceProvider.CreateScope())
                {
                    UpdateDatabase(scope.ServiceProvider, null);
                }
            });
#else
            fluentConfig.ExposeConfiguration(cfg => ());
#endif

            return fluentConfig.BuildSessionFactory();
        }

        public ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _sessionFactory?.Dispose();
                _disposed = true;
            }
        }
        private static IServiceProvider CreateServices(string connectionString)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddOracle12CManaged()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(InitialMigration).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole().AddConsole())
                .BuildServiceProvider(true);
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider, long? version)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            if (version.HasValue)
            {
                runner.MigrateDown(version.Value);
            }
            else
            {
                runner.MigrateUp();
            }
        }

        public IStatelessSession OpenStatelessSesion()
        {
            return SessionFactory.OpenStatelessSession();
        }
    }
}
