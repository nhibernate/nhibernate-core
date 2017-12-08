using NHibernate.Cfg.Loquacious;
using NHibernate.Driver;

namespace NHibernate.Cfg
{
	public static class ConnectionConfigurationExtensionFirebird
	{
		public static IConnectionConfiguration ByFirebirdDriver(this IConnectionConfiguration cfg)
		{
			return cfg.By<FirebirdDriver>();
		}

		public static void FirebirdDriver(this IDbIntegrationConfigurationProperties cfg)
		{
			cfg.Driver<FirebirdDriver>();
		}
	}
}
