using System;
using System.Collections;
using System.Data;

using NHibernate.Driver;
using NHibernate.Util;

namespace NHibernate.Connection
{
	/// <summary>
	/// The base class for the ConnectionProvider.
	/// </summary>
	public abstract class ConnectionProvider : IConnectionProvider
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ConnectionProvider));
		private string connString = null;
		protected IDriver driver = null;

		public virtual void CloseConnection(IDbConnection conn) 
		{
			log.Debug("Closing connection");
			try 
			{
				conn.Close();
			} 
			catch(Exception e) 
			{
				throw new ADOException("Could not close " + conn.GetType().ToString() + " connection", e);
			}
		}
		
		/// <summary>
		/// Configures the ConnectionProvider with the Driver and the ConnectionString.
		/// </summary>
		/// <param name="settings">A name/value Dictionary that contains the settings for this ConnectionProvider.</param>
		/// <exception cref="HibernateException">Thrown when a ConnectionString could not be found or the Driver Class could not be loaded.</exception>
		public virtual void Configure(IDictionary settings) 
		{
			log.Info("Configuring ConnectionProvider");
			connString = Cfg.Environment.Properties[ Cfg.Environment.ConnectionString ] as string;
			if (connString==null) throw new HibernateException("Could not find connection string setting");
			
			ConfigureDriver(settings);
			
		}

		/// <summary>
		/// Configures the driver for the ConnectionProvider.
		/// </summary>
		/// <param name="settings">A name/value Dictionary that contains the settings for the Driver.</param>
		protected virtual void ConfigureDriver(IDictionary settings) 
		{
			string driverClass = Cfg.Environment.Properties[ Cfg.Environment.ConnectionDriver] as string;
			if(driverClass==null) 
			{
				throw new HibernateException("The " + Cfg.Environment.ConnectionDriver + " must be specified in the NHibernate configuration section.");
			}
			else 
			{
				try 
				{
					driver = (IDriver) Activator.CreateInstance(System.Type.GetType(driverClass));
				}
				catch (Exception e) 
				{
					throw new HibernateException("Could not create the driver from " + driverClass + ".", e);
				}

			}
		}

		protected virtual string ConnectionString 
		{
			get { return connString;}
		}

		public IDriver Driver 
		{
			get {return driver;}
		}

		/// <summary>
		/// Grab a Connection from this ConnectionProvider
		/// </summary>
		/// <returns></returns>
		public abstract IDbConnection GetConnection(); 

		public abstract bool IsStatementCache {get;}

	}
}
