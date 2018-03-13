using NHibernate.Cfg.Loquacious;
using NHibernate.Driver;

namespace NHibernate.Cfg
{
	public static class ConnectionConfigurationExtensionSqlServerCompact
	{
		public static IConnectionConfiguration BySqlServerCompactDriver(this IConnectionConfiguration cfg)
		{
			return cfg.By<SqlServerCompactDriver>();
		}

		public static void SqlServerCompactDriver(this IDbIntegrationConfigurationProperties cfg)
		{
			cfg.Driver<SqlServerCompactDriver>();
		}
	}
}
