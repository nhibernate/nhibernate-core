using System;
using System.Data;
using System.Collections;
using System.Configuration;

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

		public static IConnectionProvider NewConnectionProvider(ConnectionProviderSettings settings) {
			IConnectionProvider connections = null;
			
			if (settings.Type != null) {
				try {
					log.Info("Intitializing connection provider: " + settings.Type);
					connections = (IConnectionProvider) Activator.CreateInstance(settings.Type);
				} catch (Exception e) {
					log.Fatal("Could not instantiate connection provider", e);
					throw new HibernateException("Could not instantiate connection provider: " + settings.Type.FullName);
				}
			}
			connections.Configure(settings);
			return connections;
		}

	}
}
