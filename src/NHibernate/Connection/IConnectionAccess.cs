using System.Data.Common;

namespace NHibernate.Connection
{
	//JdbcConnectionAccess.java in hibernate
	/// <summary>
	/// Provides centralized access to connections.  Centralized to hide the complexity of accounting for contextual
	/// (multi-tenant) versus non-contextual access.
	/// </summary>
	public partial interface IConnectionAccess
	{
		//ObtainConnection in hibernate
		DbConnection GetConnection();

		//ReleaseConnection in hibernate
		void CloseConnection(DbConnection conn);

		string ConnectionString { get; }
	}
}
