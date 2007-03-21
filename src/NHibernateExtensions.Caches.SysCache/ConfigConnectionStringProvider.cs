#region Using Statements
using System;
using System.Configuration;
using NHibernateExtensions.Caches.SysCache.Properties;
#endregion


namespace NHibernateExtensions.Caches.SysCache {
	/// <summary>
	/// Connection string provider that uses the ConfigurationManager to retrieve conenction strings
	/// </summary>
	public class ConfigConnectionStringProvider : IConnectionStringProvider {
		
		#region Private Fields
		/// <summary>the default connection settings</summary>
		ConnectionStringSettings _defaultConnectionSettings;
		#endregion
		
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ConfigConnectionStringProvider"/> class.
		/// </summary>
		/// <exception cref="ConfigurationErrorsException">Thrown if there are no connection strings
		/// in the application configuration file.</exception>
		public ConfigConnectionStringProvider(){
			//get the 1st connections string in the list
			this._defaultConnectionSettings = ConfigurationManager.ConnectionStrings[0];
			
			if(this._defaultConnectionSettings == null){
				throw new ConfigurationErrorsException(Resources.ConnectionStringNotConfigured);
			}
		}
		#endregion
	
		#region Public Properties
		/// <summary>
		/// Gets the name of the default connection string
		/// </summary>
		/// <value></value>
		public string DefaultConnectionName {
			get {
				return this._defaultConnectionSettings.Name;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Gets the default connection string
		/// </summary>
		public string GetConnectionString() {
			return this._defaultConnectionSettings.ConnectionString;
		}

		/// <summary>
		/// Gets a connnection string by name
		/// </summary>
		/// <param name="name">The name of the connection string to get</param>
		/// <exception cref="ConfigurationErrorsException">thorwn if the connection specified by <paramref name="name"/>
		///		could not be found.</exception>
		public string GetConnectionString(string name) {
			ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings[name];
			
			if(connectionSettings == null){
				throw new ConfigurationErrorsException(String.Format(Resources.Culture, 
					Resources.NamedConnectionStringNotFound, name));
			}
			
			return connectionSettings.ConnectionString;
		}
		#endregion
	}
}
