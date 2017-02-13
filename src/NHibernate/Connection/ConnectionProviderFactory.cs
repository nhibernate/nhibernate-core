using System;
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
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ConnectionProviderFactory));

		// cannot be instantiated
		public static IConnectionProvider NewConnectionProvider(IDictionary<string, string> settings)
		{
			IConnectionProvider connections;
			string providerClass;
			if (settings.TryGetValue(Environment.ConnectionProvider, out providerClass))
			{
				try
				{
					log.Info("Initializing connection provider: " + providerClass);
					connections =
						(IConnectionProvider)
						Environment.BytecodeProvider.ObjectsFactory.CreateInstance(ReflectHelper.ClassForName(providerClass));
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