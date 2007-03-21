#region Using Statements
using System;
using System.Data.SqlClient;
using System.Data;
using System.Web.Caching;
#endregion

namespace NHibernateExtensions.Caches.SysCache {
	
	/// <summary>
	/// Creates SqlCacheDependency objects dependent on data changes in a table and registers the dependency for 
	/// change notifications if necessary
	/// </summary>
	public class SqlTableCacheDependencyEnlister : ICacheDependencyEnlister{
	
		#region Private Fields
		/// <summary>the name of the table to monitor</summary>
		private string _tableName;
		/// <summary>the name of the database entry to use for connection info</summary>
		private string _databaseEntryName;
		#endregion
		
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlTableCacheDependencyEnlister"/> class.
		/// </summary>
		/// <param name="tableName">Name of the table to monitor</param>
		/// <param name="databaseEntryName">The name of the database entry to use for connection information</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="tableName"/> or 
		///		<paramref name="databaseEntryName"/> is null or empty.</exception>
		public SqlTableCacheDependencyEnlister(string tableName, string databaseEntryName){
			//validate the params
			if(String.IsNullOrEmpty(tableName)){
				throw new ArgumentNullException("tableName");
			}
			
			if(String.IsNullOrEmpty(databaseEntryName)){
				throw new ArgumentNullException("databaseEntryName");
			}
			
			this._tableName = tableName;
			this._databaseEntryName = databaseEntryName;
			
		}
		#endregion
		
		#region Public Methods
		/// <summary>
		/// Enlists a cache dependency to recieve change notifciations with an underlying resource
		/// </summary>
		/// <returns>
		/// The cache dependency linked to the notification subscription
		/// </returns>
		public CacheDependency Enlist() {
			//there is no need to enlist in notification subscription for this type of dependency
			
			return new SqlCacheDependency(this._databaseEntryName, this._tableName);
		}
		#endregion
	}
}
