using System.Collections.Generic;

using NHibernate.Util;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Connection
{
	/// <summary>
	/// Instantiates a connection provider given configuration properties.
	/// </summary>
	public static class ConnectionProviderFactory
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(ConnectionProviderFactory));

		public static IConnectionProvider NewConnectionProvider(IDictionary<string, string> settings)
		{
			var defaultConnectionProvider =
				settings.ContainsKey(Environment.ConnectionString) ||
				settings.ContainsKey(Environment.ConnectionStringName)
					? typeof(DriverConnectionProvider)
					: typeof(UserSuppliedConnectionProvider);
			var connectionProvider = PropertiesHelper.GetInstance<IConnectionProvider>(
				Environment.ConnectionProvider,
				settings,
				defaultConnectionProvider);
			log.Info("Connection provider: '{0}'", connectionProvider.GetType().AssemblyQualifiedName);
			connectionProvider.Configure(settings);
			return connectionProvider;
		}
	}
}
