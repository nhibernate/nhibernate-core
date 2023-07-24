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
			NHibernateLogger.SetLoggersFactory(new NHibernateToMicrosoftLoggerFactory(loggerFactory));

			var mapper = new ModelMapper();
			mapper.AddMapping<ItemMap>();
			var domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
			
			Configuration = new Configuration();
			Configuration.DataBaseIntegration(db =>
				{
					db.ConnectionString = @"Data Source=nhibernate.db;DateTimeFormatString=yyyy-MM-dd HH:mm:ss.FFFFFFF;";
					db.Dialect<SQLiteDialect>();
					db.Driver<SQLite20Driver>();
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
