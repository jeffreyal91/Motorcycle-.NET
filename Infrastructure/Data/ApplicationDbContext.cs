using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Mapping.ByCode;
using System;
using Domain.Entities; 

namespace Infrastructure.Data
{
    public class ApplicationDbContext
    {
        private readonly ISessionFactory _sessionFactory;

        public ApplicationDbContext()
        {
            _sessionFactory = BuildSessionFactory();
        }

        private ISessionFactory BuildSessionFactory()
        {
            return Fluently.Configure()
                .Database(PostgreSQLConfiguration.Standard.ConnectionString(c => c.Is("YourConnectionStringHere")))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Motorcycle>())
                .BuildSessionFactory();
        }

        public ISession OpenSession()
        {
            return _sessionFactory.OpenSession();
        }
    }
}
