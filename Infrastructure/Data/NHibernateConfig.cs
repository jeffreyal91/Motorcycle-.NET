using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Infrastructure.Data.Mappings;
using Infrastructure.Data.NHibernate.Mappings;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Infrastructure.Data
{
    public static class NHibernateConfig
    {
        private static ISessionFactory? _sessionFactory;
        private static readonly object _lock = new object();
        
        /// <summary>
        /// Singleton session factory instance
        /// </summary>
        public static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    throw new InvalidOperationException("NHibernate not initialized. Call Initialize() first.");
                }
                return _sessionFactory;
            }
        }

        /// <summary>
        /// Initializes NHibernate with the provided configuration
        /// </summary>
        /// <param name="configuration">Application configuration</param>
        public static void Initialize(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            lock (_lock)
            {
                if (_sessionFactory == null)
                {
                    var connectionString = configuration.GetConnectionString("PostgreSQL") 
                        ?? throw new InvalidOperationException("PostgreSQL connection string is missing in configuration");
                    
                    _sessionFactory = BuildSessionFactory(connectionString);
                }
            }
        }

        /// <summary>
        /// Creates and configures a new NHibernate session factory
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <returns>Configured session factory</returns>
        public static ISessionFactory CreateSessionFactory(string connectionString)
        {
            return BuildSessionFactory(connectionString);
        }

        private static ISessionFactory BuildSessionFactory(string connectionString)
        {
            return Fluently.Configure()
                .Database(PostgreSQLConfiguration.PostgreSQL82
                    .ConnectionString(connectionString)
                    .FormatSql()
                    .ShowSql()
                )
                .Mappings(m => m.FluentMappings
                    .AddFromAssemblyOf<MotorcycleMap>()
                    .AddFromAssemblyOf<DeliveryDriverMap>() 
                    .AddFromAssemblyOf<MotorcycleEventMap>() 
                    .AddFromAssemblyOf<RentalMap>() 
                )
                .ExposeConfiguration(cfg => 
                {
                    new SchemaUpdate(cfg).Execute(false, true);
                    cfg.SetProperty("hbm2ddl.keywords", "auto-quote");
                    cfg.SetProperty("hibernate.default_schema", "public");
                })
                .BuildSessionFactory();
        }

        /// <summary>
        /// Opens a new NHibernate session
        /// </summary>
        /// <returns>New database session</returns>
        /// <exception cref="InvalidOperationException">Thrown if session factory is not initialized</exception>
        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}