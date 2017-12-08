using NHibernate.Cfg.Loquacious;
using NHibernate.Driver;

namespace NHibernate.Cfg
{
	public static class ConnectionConfigurationExtensionSQLite
	{
		public static IConnectionConfiguration BySQLiteDriver(this IConnectionConfiguration cfg)
		{
			return cfg.By<SQLiteDriver>();
		}

		public static void SQLiteDriver(this IDbIntegrationConfigurationProperties cfg)
		{
			cfg.Driver<SQLiteDriver>();
		}
	}
}
