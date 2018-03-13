using NHibernate.Cfg.Loquacious;
using NHibernate.Driver;

namespace NHibernate.Cfg
{
	public static class ConnectionConfigurationExtensionOracleManaged
	{
		public static IConnectionConfiguration ByOracleManagedDriver(this IConnectionConfiguration cfg)
		{
			return cfg.By<OracleManagedDriver>();
		}

		public static void OracleManagedDriver(this IDbIntegrationConfigurationProperties cfg)
		{
			cfg.Driver<OracleManagedDriver>();
		}
	}
}
