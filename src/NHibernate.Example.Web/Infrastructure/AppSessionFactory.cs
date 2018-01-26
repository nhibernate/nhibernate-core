using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Example.Web.Models;
using NHibernate.Mapping.ByCode;

namespace NHibernate.Example.Web.Infrastructure
{
	public class AppSessionFactory
	{
		public Configuration Configuration { get; }
		public ISessionFactory SessionFactory { get; }

		public AppSessionFactory(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
		{
			NHibernate.NHibernateLogger.SetLoggersFactory(new NHibernateToMicrosoftLoggerFactory(loggerFactory));

			var mapper = new ModelMapper();
			mapper.AddMapping<ItemMap>();
			var domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
			
			Configuration = new Configuration();
			Configuration.DataBaseIntegration(db =>
				{
					db.ConnectionString = @"Server=(local)\SQLEXPRESS;initial catalog=nhibernate;Integrated Security=true";
					db.Dialect<MsSql2008Dialect>();
					db.SqlServer2008Driver();
				})
				.AddMapping(domainMapping);
			Configuration.SessionFactory().GenerateStatistics();

			SessionFactory = Configuration.BuildSessionFactory();
		}

		public ISession OpenSession()
		{
			return SessionFactory.OpenSession();
		}
	}
}
