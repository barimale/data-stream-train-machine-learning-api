using Demo.Infrastructure.Database;
using Demo.Infrastructure.Database.Interceptors;
using Demo.Infrastructure.EntityConfigurations;
using Demo.Migrations.Conventions;
using Demo.Migrations.Migrations;
using FluentMigrator.Runner;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHibernate;
using ISession = NHibernate.ISession;

namespace Demo.Infrastructure
{
    public class InOneFileNHibernateHelper : INHibernateHelper
    {
        private string _connectionString = "Data Source=databasename.db;Cache=Shared;Foreign Keys=True;"; //Mode=Memory
        private static readonly object _lock = new();
        private ISessionFactory? _sessionFactory;
        private bool _disposed;

        public InOneFileNHibernateHelper()
        {
            //intentionally left blank
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
                .Database(SQLiteConfiguration.Standard
                    //.InMemory()
                    .ConnectionString(_connectionString)
                    .Driver<NHibernate.Driver.SQLite20Driver>()
                    .Dialect<NHibernate.Dialect.SQLiteDialect>())
                .Mappings(m =>
                {
                    m.FluentMappings.AddFromAssemblyOf<DataMap>().Conventions.Add<LowercaseTableNameConvention>();
                });
            fluentConfig.ExposeConfiguration(cfg =>
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Information);
                    builder.AddConsole();
                });

                cfg.SetInterceptor(new NHibernateInterceptor(loggerFactory));
            });

            var serviceProvider = CreateServices(_connectionString);

            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider, null);
            }

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
                    .AddSQLite()
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
