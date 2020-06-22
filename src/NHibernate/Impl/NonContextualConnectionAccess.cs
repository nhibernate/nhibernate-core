using System;
using System.Data.Common;
using NHibernate.Connection;

namespace NHibernate.Impl
{
	[Serializable]
	partial class NonContextualConnectionAccess : IConnectionAccess
	{
		public NonContextualConnectionAccess(string connectionString)
		{
			ConnectionString = connectionString;
		}

		public DbConnection GetConnection(IConnectionProvider provider)
		{
			return provider.GetConnection(ConnectionString);
		}

		public void CloseConnection(DbConnection conn, IConnectionProvider provider)
		{
			provider.CloseConnection(conn);
		}

		public string ConnectionString { get; }
	}
}
