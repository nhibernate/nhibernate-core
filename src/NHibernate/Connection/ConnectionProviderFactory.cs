using System;
using System.Data;
using System.Collections;
using System.Configuration;
using NHibernate.Cfg;

namespace NHibernate.Connection {

	/// <summary>
	/// Instanciates a connection provider given configuration properties.
	/// </summary>
	public class ConnectionProviderFactory {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ConnectionProviderFactory));
		
		// cannot be instantiated
		private ConnectionProviderFactory() {
			throw new InvalidOperationException();
		}

		public static IConnectionProvider NewConnectionProvider(IDictionary settings) {
			IConnectionProvider connections = null;
			string providerClass = settings[Cfg.Environment.ConnectionProvider] as string;
			if (providerClass != null) {
				try {
					log.Info("Intitializing connection provider: " + providerClass);
					connections = (IConnectionProvider) Activator.CreateInstance(System.Type.GetType(providerClass));
				} catch (Exception e) {
					log.Fatal("Could not instantiate connection provider", e);
					throw new HibernateException("Could not instantiate connection provider: " + providerClass);
				}
			} else {
				connections = new UserSuppliedConnectionProvider();
			}
			connections.Configure(settings);
			return connections;
		}

	}
}
