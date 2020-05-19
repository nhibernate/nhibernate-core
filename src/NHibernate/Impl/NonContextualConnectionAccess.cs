using System;
using System.Data.Common;
using NHibernate.Connection;
using NHibernate.Engine;

namespace NHibernate.Impl
{
	[Serializable]
	partial class NonContextualConnectionAccess : IConnectionAccess
	{
		private readonly ISessionFactoryImplementor _factory;

		public NonContextualConnectionAccess(ISessionFactoryImplementor factory)
		{
			_factory = factory;
		}

		public DbConnection GetConnection()
		{
			return _factory.ConnectionProvider.GetConnection();
		}

		public void CloseConnection(DbConnection connection)
		{
			_factory.ConnectionProvider.CloseConnection(connection);
		}

		public string ConnectionString => _factory.ConnectionProvider.GetConnectionString();
	}
}
