#region Using Statements
using System;
using System.Data.SqlClient;
using System.Data;
using System.Web.Caching;
using NHibernateExtensions.Caches.SysCache;
#endregion

namespace NHibernateExtensions.Caches.SysCache {

	/// <summary>
	/// Creates SqlCacheDependency objects and hooks them up to a query notification based on the command
	/// </summary>
	public class SqlCommandCacheDependencyEnlister : ICacheDependencyEnlister {
	
		#region Private Fields
		/// <summary>sql command to use for creating notifications</summary>
		private string _command;
		/// <summary>indicates if the command is a stored procedure or not</summary>
		private bool _isStoredProcedure;
		/// <summary>The name of the connection string</summary>
		private string _connectionName;
		/// <summary>The connection string to use for connection to the date source</summary>
		private string _connectionString;
		#endregion
	
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlCommandCacheDependencyEnlister"/> class.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="isStoredProcedure">if set to <c>true</c> [is stored procedure].</param>
		/// <param name="connectionStringProvider">The <see cref="IConnectionStringProvider"/> to use 
		///		to retrieve the connection string to connect to the underlying data store and enlist in query notifications</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="command"/> or 
		///		<paramref name="connectionStringProvider"/> is null or empty.</exception>
		public SqlCommandCacheDependencyEnlister(string command, bool isStoredProcedure, 
			IConnectionStringProvider connectionStringProvider):this(command, isStoredProcedure, null, connectionStringProvider){
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlCommandCacheDependencyEnlister"/> class.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="isStoredProcedure">if set to <c>true</c> [is stored procedure].</param>
		/// <param name="connectionName">Name of the connection.</param>
		/// <param name="connectionStringProvider">The <see cref="IConnectionStringProvider"/> to use 
		///		to retrieve the connection string to connect to the underlying data store and enlist in query notifications</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="command"/> or 
		///		<paramref name="connectionStringProvider"/> is null or empty.</exception>
		public SqlCommandCacheDependencyEnlister(string command, bool isStoredProcedure, 
			string connectionName, IConnectionStringProvider connectionStringProvider){
			
			//validate the parameters
			if(String.IsNullOrEmpty(command)){
				throw new ArgumentNullException("command");
			}
			
			if(connectionStringProvider == null){
				throw new ArgumentNullException("connectionStringProvider");
			}
						
			this._command = command;
			this._isStoredProcedure = isStoredProcedure;
			this._connectionName = connectionName;
			
			if(String.IsNullOrEmpty(this._connectionName)){
				this._connectionString = connectionStringProvider.GetConnectionString();
			}else{
				this._connectionString = connectionStringProvider.GetConnectionString(this._connectionName);
			}
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
			SqlCacheDependency dependency;
			
			//setup and execute the command that will register the cache dependency for
			//change notifications
			using(SqlConnection connection = new SqlConnection(this._connectionString)){
				using(SqlCommand command = new SqlCommand(this._command, connection)){
					
					//is the command a sproc
					if(this._isStoredProcedure){
						command.CommandType = CommandType.StoredProcedure;
					}
					
					//hook the deondency up to the command
					dependency = new SqlCacheDependency(command);
					
					connection.Open();
					//execute the query, this will enlist the dependency. Notice that we execute a non query since
					//we dont need any results
					command.ExecuteNonQuery();
				}
			}
			
			return dependency;
		}
		#endregion
	}
}
