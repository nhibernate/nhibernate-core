using System;
using System.Collections;
using log4net;

using NHibernate.Util;

using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Connection
{
	/// <summary>
	/// Instanciates a connection provider given configuration properties.
	/// </summary>
	public sealed class ConnectionProviderFactory
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( ConnectionProviderFactory ) );

		// cannot be instantiated
		private ConnectionProviderFactory()
		{
			throw new InvalidOperationException( "ConnectionProviderFactory can not be instantiated." );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static IConnectionProvider NewConnectionProvider( IDictionary settings )
		{
			IConnectionProvider connections = null;
			string providerClass = settings[ Environment.ConnectionProvider ] as string;
			if( providerClass != null )
			{
				try
				{
					log.Info( "Intitializing connection provider: " + providerClass );
					connections = ( IConnectionProvider ) Activator.CreateInstance( ReflectHelper.ClassForName( providerClass ) );
				}
				catch( Exception e )
				{
					log.Fatal( "Could not instantiate connection provider", e );
					throw new HibernateException( "Could not instantiate connection provider: " + providerClass );
				}
			}
			else
			{
				connections = new UserSuppliedConnectionProvider();
			}
			connections.Configure( settings );
			return connections;
		}

	}
}