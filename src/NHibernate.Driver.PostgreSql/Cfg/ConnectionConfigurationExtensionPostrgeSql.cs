using NHibernate.Cfg.Loquacious;
using NHibernate.Driver;

namespace NHibernate.Cfg
{
	public static class ConnectionConfigurationExtensionPostrgeSql
	{
		public static IConnectionConfiguration ByPostgreSqlDriver(this IConnectionConfiguration cfg)
		{
			return cfg.By<PostgreSqlDriver>();
		}

		public static void PostgreSqlDriver(this IDbIntegrationConfigurationProperties cfg)
		{
			cfg.Driver<PostgreSqlDriver>();
		}
	}
}
