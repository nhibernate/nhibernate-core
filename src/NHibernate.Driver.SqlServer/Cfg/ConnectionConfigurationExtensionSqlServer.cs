using NHibernate.Cfg.Loquacious;
using NHibernate.Driver;

namespace NHibernate.Cfg
{
	public static class ConnectionConfigurationExtensionSqlServer
	{
		public static IConnectionConfiguration BySqlServer2000Driver(this IConnectionConfiguration cfg)
		{
			return cfg.By<SqlServer2000Driver>();
		}

		public static void SqlServer2000Driver(this IDbIntegrationConfigurationProperties cfg)
		{
			cfg.Driver<SqlServer2000Driver>();
		}

		public static IConnectionConfiguration BySqlServer2008Driver(this IConnectionConfiguration cfg)
		{
			return cfg.By<SqlServer2008Driver>();
		}

		public static void SqlServer2008Driver(this IDbIntegrationConfigurationProperties cfg)
		{
			cfg.Driver<SqlServer2008Driver>();
		}
	}
}
