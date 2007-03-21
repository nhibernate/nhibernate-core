#region Using Statements
using System;
#endregion

namespace NHibernateExtensions.Caches.SysCache {

	/// <summary>
	/// Provides connection strings
	/// </summary>
	public interface IConnectionStringProvider {
	
		/// <summary>
		/// Gets the name of the default connection string
		/// </summary>
		string DefaultConnectionName{get;}
		
		/// <summary>
		/// Gets the default connection string
		/// </summary>
		string GetConnectionString();
		
		/// <summary>
		/// Gets a connnection string by name
		/// </summary>
		/// <param name="name">The name of the connection string to get</param>
		string GetConnectionString(string name);
	}
}
