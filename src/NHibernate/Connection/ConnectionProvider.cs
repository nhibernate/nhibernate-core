using System;
using System.Collections;
using System.Data;
using log4net;
using NHibernate.Driver;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Connection
{
	/// <summary>
	/// The base class for the ConnectionProvider.
	/// </summary>
	public abstract class ConnectionProvider : IConnectionProvider, IDisposable
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( ConnectionProvider ) );
		private string connString = null;
		private IDriver driver = null;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="conn"></param>
		public virtual void CloseConnection( IDbConnection conn )
		{
			log.Debug( "Closing connection" );
			try
			{
				conn.Close();
			}
			catch( Exception e )
			{
				throw new ADOException( "Could not close " + conn.GetType().ToString() + " connection", e );
			}
		}

		/// <summary>
		/// Configures the ConnectionProvider with the Driver and the ConnectionString.
		/// </summary>
		/// <param name="settings">A name/value Dictionary that contains the settings for this ConnectionProvider.</param>
		/// <exception cref="HibernateException">Thrown when a ConnectionString could not be found or the Driver Class could not be loaded.</exception>
		public virtual void Configure( IDictionary settings )
		{
			log.Info( "Configuring ConnectionProvider" );

			connString = settings[ Environment.ConnectionString ] as string;
			if( connString == null )
			{
				throw new HibernateException( "Could not find connection string setting" );
			}

			ConfigureDriver( settings );

		}

		/// <summary>
		/// Configures the driver for the ConnectionProvider.
		/// </summary>
		/// <param name="settings">A name/value Dictionary that contains the settings for the Driver.</param>
		protected virtual void ConfigureDriver( IDictionary settings )
		{
			string driverClass = settings[ Environment.ConnectionDriver ] as string;
			if( driverClass == null )
			{
				throw new HibernateException( "The " + Environment.ConnectionDriver + " must be specified in the NHibernate configuration section." );
			}
			else
			{
				try
				{
					driver = ( IDriver ) Activator.CreateInstance( System.Type.GetType( driverClass ) );
				}
				catch( Exception e )
				{
					throw new HibernateException( "Could not create the driver from " + driverClass + ".", e );
				}

			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual string ConnectionString
		{
			get { return connString; }
		}

		/// <summary>
		/// 
		/// </summary>
		public IDriver Driver
		{
			get { return driver; }
		}

		/// <summary>
		/// Grab a Connection from this ConnectionProvider
		/// </summary>
		/// <returns></returns>
		public abstract IDbConnection GetConnection();

		/// <summary>
		/// 
		/// </summary>
		public abstract bool IsStatementCache { get; }

		/// <summary>
		/// 
		/// </summary>
		public abstract void Close();

		#region IDisposable Members

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			Close();
		}

		#endregion
	}
}