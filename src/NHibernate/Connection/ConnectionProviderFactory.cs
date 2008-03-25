using System;
using System.Collections.Generic;
using log4net;
using NHibernate.Util;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Connection
{
	/// <summary>
	/// Instanciates a connection provider given configuration properties.
	/// </summary>
	public sealed class ConnectionProviderFactory
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ConnectionProviderFactory));

		// cannot be instantiated
		private ConnectionProviderFactory()
		{
			throw new InvalidOperationException("ConnectionProviderFactory can not be instantiated.");
		}

		public static IConnectionProvider NewConnectionProvider(IDictionary<string, string> settings)
		{
			IConnectionProvider connections;
			string providerClass;
			if (settings.TryGetValue(Environment.ConnectionProvider, out providerClass))
			{
				try
				{
					log.Info("Initializing connection provider: " + providerClass);
					connections = (IConnectionProvider) Activator.CreateInstance(ReflectHelper.ClassForName(providerClass));
				}
				catch (Exception e)
				{
					log.Fatal("Could not instantiate connection provider", e);
					throw new HibernateException("Could not instantiate connection provider: " + providerClass, e);
				}
			}
			else if (settings.ContainsKey(Environment.ConnectionString) || settings.ContainsKey(Environment.ConnectionStringName))
			{
				connections = new DriverConnectionProvider();
			}
			else
			{
				log.Info("No connection provider specified, UserSuppliedConnectionProvider will be used.");
				connections = new UserSuppliedConnectionProvider();
			}
			connections.Configure(settings);
			return connections;
		}
	}
}