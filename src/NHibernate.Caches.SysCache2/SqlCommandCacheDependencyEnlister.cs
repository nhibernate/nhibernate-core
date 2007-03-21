using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Caching;

namespace NHibernateExtensions.Caches.SysCache
{
	/// <summary>
	/// Creates SqlCacheDependency objects and hooks them up to a query notification based on the command
	/// </summary>
	public class SqlCommandCacheDependencyEnlister : ICacheDependencyEnlister
	{
		/// <summary>sql command to use for creating notifications</summary>
		private string _command;

		/// <summary>indicates if the command is a stored procedure or not</summary>
		private bool _isStoredProcedure;

		/// <summary>The name of the connection string</summary>
		private string _connectionName;

		/// <summary>The connection string to use for connection to the date source</summary>
		private string _connectionString;

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
		                                         IConnectionStringProvider connectionStringProvider)
			: this(command, isStoredProcedure, null, connectionStringProvider)
		{
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
		                                         string connectionName, IConnectionStringProvider connectionStringProvider)
		{
			//validate the parameters
			if (String.IsNullOrEmpty(command))
			{
				throw new ArgumentNullException("command");
			}

			if (connectionStringProvider == null)
			{
				throw new ArgumentNullException("connectionStringProvider");
			}

			_command = command;
			_isStoredProcedure = isStoredProcedure;
			_connectionName = connectionName;

			if (String.IsNullOrEmpty(_connectionName))
			{
				_connectionString = connectionStringProvider.GetConnectionString();
			}
			else
			{
				_connectionString = connectionStringProvider.GetConnectionString(_connectionName);
			}
		}

		#region ICacheDependencyEnlister Members

		/// <summary>
		/// Enlists a cache dependency to recieve change notifciations with an underlying resource
		/// </summary>
		/// <returns>
		/// The cache dependency linked to the notification subscription
		/// </returns>
		public CacheDependency Enlist()
		{
			SqlCacheDependency dependency;

			//setup and execute the command that will register the cache dependency for
			//change notifications
			using (SqlConnection connection = new SqlConnection(_connectionString))
			using (SqlCommand command = new SqlCommand(_command, connection))
			{
				//is the command a sproc
				if (_isStoredProcedure)
				{
					command.CommandType = CommandType.StoredProcedure;
				}

				//hook the deondency up to the command
				dependency = new SqlCacheDependency(command);

				connection.Open();
				//execute the query, this will enlist the dependency. Notice that we execute a non query since
				//we dont need any results
				command.ExecuteNonQuery();
			}

			return dependency;
		}

		#endregion
	}
}