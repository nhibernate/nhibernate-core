using System;
using System.Collections;
using System.Configuration;
using System.Data.Common;

using NHibernate.Driver;
using NHibernate.Util;
using Environment=NHibernate.Cfg.Environment;
using System.Collections.Generic;

namespace NHibernate.Connection
{
	/// <summary>
	/// The base class for the ConnectionProvider.
	/// </summary>
	public abstract class ConnectionProvider : IConnectionProvider
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ConnectionProvider));
		private string connString;
		private IDriver driver;

		/// <summary>
		/// Closes the <see cref="DbConnection"/>.
		/// </summary>
		/// <param name="conn">The <see cref="DbConnection"/> to clean up.</param>
		public virtual void CloseConnection(DbConnection conn)
		{
			log.Debug("Closing connection");
			try
			{
				conn.Close();
			}
			catch (Exception e)
			{
				throw new ADOException("Could not close " + conn.GetType() + " connection", e);
			}
		}

		/// <summary>
		/// Configures the ConnectionProvider with the Driver and the ConnectionString.
		/// </summary>
		/// <param name="settings">An <see cref="IDictionary"/> that contains the settings for this ConnectionProvider.</param>
		/// <exception cref="HibernateException">
		/// Thrown when a <see cref="Cfg.Environment.ConnectionString"/> could not be found 
		/// in the <c>settings</c> parameter or the Driver Class could not be loaded.
		/// </exception>
		public virtual void Configure(IDictionary<string, string> settings)
		{
			log.Info("Configuring ConnectionProvider");

			// Connection string in the configuration overrides named connection string
			if (!settings.TryGetValue(Environment.ConnectionString, out connString))
				connString = GetNamedConnectionString(settings);

			if (connString == null)
			{
				throw new HibernateException("Could not find connection string setting (set " +
				                             Environment.ConnectionString +
				                             " or " +
				                             Environment.ConnectionStringName +
				                             " property)");
			}

			ConfigureDriver(settings);
		}

		/// <summary>
		/// Get the .NET 2.0 named connection string 
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when a <see cref="Environment.ConnectionStringName"/> was found 
		/// in the <c>settings</c> parameter but could not be found in the app.config
		/// </exception>
		protected virtual string GetNamedConnectionString(IDictionary<string, string> settings)
		{
			string connStringName;
			if(!settings.TryGetValue(Environment.ConnectionStringName, out connStringName))
				return null;

			ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connStringName];
			if (connectionStringSettings == null)
				throw new HibernateException(string.Format("Could not find named connection string {0}", connStringName));
			return connectionStringSettings.ConnectionString;
		}

		/// <summary>
		/// Configures the driver for the ConnectionProvider.
		/// </summary>
		/// <param name="settings">An <see cref="IDictionary"/> that contains the settings for the Driver.</param>
		/// <exception cref="HibernateException">
		/// Thrown when the <see cref="Environment.ConnectionDriver"/> could not be 
		/// found in the <c>settings</c> parameter or there is a problem with creating
		/// the <see cref="IDriver"/>.
		/// </exception>
		protected virtual void ConfigureDriver(IDictionary<string, string> settings)
		{
			string driverClass;
			if (!settings.TryGetValue(Environment.ConnectionDriver, out driverClass))
			{
				throw new HibernateException("The " + Environment.ConnectionDriver +
				                             " must be specified in the NHibernate configuration section.");
			}
			else
			{
				try
				{
					driver =
						(IDriver) Environment.BytecodeProvider.ObjectsFactory.CreateInstance(ReflectHelper.ClassForName(driverClass));
					driver.Configure(settings);
				}
				catch (Exception e)
				{
					throw new HibernateException("Could not create the driver from " + driverClass + ".", e);
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="String"/> for the <see cref="DbConnection"/>
		/// to connect to the database.
		/// </summary>
		/// <value>
		/// The <see cref="String"/> for the <see cref="DbConnection"/>
		/// to connect to the database.
		/// </value>
		protected virtual string ConnectionString
		{
			get { return connString; }
		}

		/// <summary>
		/// Gets the <see cref="IDriver"/> that can create the <see cref="DbConnection"/> object.
		/// </summary>
		/// <value>
		/// The <see cref="IDriver"/> that can create the <see cref="DbConnection"/>.
		/// </value>
		public IDriver Driver
		{
			get { return driver; }
		}

		/// <summary>
		/// Get an open <see cref="DbConnection"/>.
		/// </summary>
		/// <returns>An open <see cref="DbConnection"/>.</returns>
		public abstract DbConnection GetConnection();

		#region IDisposable Members

		/// <summary>
		/// A flag to indicate if <c>Disose()</c> has been called.
		/// </summary>
		private bool _isAlreadyDisposed;

		/// <summary>
		/// Finalizer that ensures the object is correctly disposed of.
		/// </summary>
		~ConnectionProvider()
		{
			Dispose(false);
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Takes care of freeing the managed and unmanaged resources that 
		/// this class is responsible for.
		/// </summary>
		/// <param name="isDisposing">Indicates if this ConnectionProvider is being Disposed of or Finalized.</param>
		/// <remarks>
		/// <p>
		/// If this ConnectionProvider is being Finalized (<c>isDisposing==false</c>) then make 
		/// sure not to call any methods that could potentially bring this 
		/// ConnectionProvider back to life.
		/// </p>
		/// <p>
		/// If any subclasses manage resources that also need to be disposed of this method
		/// should be overridden, but don't forget to call it in the override.
		/// </p>
		/// </remarks>
		protected virtual void Dispose(bool isDisposing)
		{
			if (_isAlreadyDisposed)
			{
				// don't dispose of multiple times.
				return;
			}

			// free managed resources that are being managed by the ConnectionProvider if we
			// know this call came through Dispose()
			if (isDisposing)
			{
				log.Debug("Disposing of ConnectionProvider.");
			}

			// free unmanaged resources here

			_isAlreadyDisposed = true;
			// nothing for Finalizer to do - so tell the GC to ignore it
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
