using NHibernate.Cfg.Loquacious;
using NHibernate.Driver;

namespace NHibernate.Cfg
{
	public static class ConnectionConfigurationExtensionMySql
	{
		public static IConnectionConfiguration ByMySqlDriver(this IConnectionConfiguration cfg)
		{
			return cfg.By<MySqlDriver>();
		}

		public static void MySqlDriver(this IDbIntegrationConfigurationProperties cfg)
		{
			cfg.Driver<MySqlDriver>();
		}
	}
}
