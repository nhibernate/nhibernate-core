using System;
using System.Data.Common;
using NHibernate.Connection;
using NHibernate.Util;

namespace NHibernate.MultiTenancy
{
	/// <summary>
	/// Base implementation for <seealso cref="MultiTenancyStrategy.Database"/> multi-tenancy strategy
	/// </summary>
	public abstract partial class AbstractMultiTenantConnectionProvider : IMultiTenantConnectionProvider
	{
		protected abstract string TenantConnectionString { get; }
		public abstract string TenantIdentifier { get; }

		public IConnectionAccess GetConnectionAccess()
		{
			//TODO 6.0: Remove check
			ReflectHelper.CastOrThrow<ConnectionProvider>(ConnectionProvider, $"multi-tenancy. For custom connection provider please implement IMultiTenantConnectionProvider directly for '{GetType().Name}'  and do not use {nameof(AbstractMultiTenantConnectionProvider)} as base class."); 
			return new ContextualConnectionAccess(TenantConnectionString, ConnectionProvider);
		}

		protected  abstract IConnectionProvider ConnectionProvider { get; }
[Serializable]

		partial class ContextualConnectionAccess : IConnectionAccess
		{
			private readonly IConnectionProvider _connectionProvider;

			public ContextualConnectionAccess(string connectionString, IConnectionProvider connectionProvider)
			{
				ConnectionString = connectionString;
				_connectionProvider = connectionProvider;
			}

			public DbConnection GetConnection()
			{
				return _connectionProvider.GetConnection(ConnectionString);
			}

			public void CloseConnection(DbConnection connection)
			{
				_connectionProvider.CloseConnection(connection);
			}

			public string ConnectionString { get; }
		}  
	}
}
