using System;
using System.Data.Common;
using NHibernate.Connection;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	/// <summary>
	/// A non contextual connection access used when multi-tenant is not enabled.  
	/// </summary>
	[Serializable]
	partial class NonContextualConnectionAccess : IConnectionAccess
	{
		private readonly ISessionFactoryImplementor _sessionFactory;

		public NonContextualConnectionAccess(ISessionFactoryImplementor connectionProvider)
		{
			_sessionFactory = connectionProvider;
		}

		/// <inheritdoc />
		public string ConnectionString => _sessionFactory.ConnectionProvider.GetConnectionString();

		/// <inheritdoc />
		public DbConnection GetConnection()
		{
			return _sessionFactory.ConnectionProvider.GetConnection();
		}

		/// <inheritdoc />
		public void CloseConnection(DbConnection connection)
		{
			_sessionFactory.ConnectionProvider.CloseConnection(connection);
		}
	}
}
