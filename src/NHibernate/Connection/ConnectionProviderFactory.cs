using System;
using System.Collections;
using System.Configuration;
using System.Data;

using NHibernate.Cfg;

namespace NHibernate.Connection 
{
	/// <summary>
	/// Instanciates a connection provider given configuration properties.
	/// </summary>
	public class ConnectionProviderFactory 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ConnectionProviderFactory));
		
		// cannot be instantiated
		private ConnectionProviderFactory() 
		{
			throw new InvalidOperationException();
		}

		public static IConnectionProvider NewConnectionProvider(IDictionary settings) 
		{
			IConnectionProvider connections = null;
			string providerClass = settings[Cfg.Environment.ConnectionProvider] as string;
			if (providerClass != null) 
			{
				try 
				{
					log.Info("Intitializing connection provider: " + providerClass);
					connections = (IConnectionProvider) Activator.CreateInstance(System.Type.GetType(providerClass));
				} 
				catch (Exception e) 
				{
					log.Fatal("Could not instantiate connection provider", e);
					throw new HibernateException("Could not instantiate connection provider: " + providerClass);
				}
			} 
			else 
			{
				throw new NotImplementedException("We have not implemented user supplied connections yet.");
//				connections = new UserSuppliedConnectionProvider();
//				connections = new SqlServerConnectionProvider();
			}
			connections.Configure(settings);
			return connections;
		}

	}
}
