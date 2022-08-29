using System.Data.Common;

namespace NHibernate.Connection
{
	//JdbcConnectionAccess.java in hibernate
	/// <summary>
	/// Provides centralized access to connections.  Centralized to hide the complexity of accounting for contextual
	/// (multi-tenant) versus non-contextual access.
	/// Implementation must be serializable
	/// </summary>
	public partial interface IConnectionAccess
	{
		/// <summary>
		/// The connection string of the database connection.
		/// </summary>
		string ConnectionString { get; }

		//ObtainConnection in hibernate
		/// <summary>
		/// Gets the database connection.
		/// </summary>
		/// <returns>The database connection.</returns>
		DbConnection GetConnection();

		//ReleaseConnection in hibernate
		/// <summary>
		/// Closes the given database connection.
		/// </summary>
		/// <param name="connection">The connection to close.</param>
		void CloseConnection(DbConnection connection);
	}
}
