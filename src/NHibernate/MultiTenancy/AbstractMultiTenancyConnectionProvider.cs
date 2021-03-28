using System;
using System.Data.Common;
using NHibernate.Connection;
using NHibernate.Engine;

namespace NHibernate.MultiTenancy
{
	/// <summary>
	/// Base implementation for multi-tenancy strategy.
	/// </summary>
	[Serializable]
	public abstract partial class AbstractMultiTenancyConnectionProvider : IMultiTenancyConnectionProvider
	{
		/// <inheritdoc />
		public IConnectionAccess GetConnectionAccess(TenantConfiguration tenantConfiguration, ISessionFactoryImplementor sessionFactory)
		{
			var tenantConnectionString = GetTenantConnectionString(tenantConfiguration, sessionFactory);
			if (string.IsNullOrEmpty(tenantConnectionString))
			{
				throw new HibernateException($"Tenant '{tenantConfiguration.TenantIdentifier}' connection string is empty.");
			}

			return new ContextualConnectionAccess(tenantConnectionString, sessionFactory);
		}

		/// <summary>
		/// Gets the connection string for the given tenant configuration.
		/// </summary>
		/// <param name="tenantConfiguration">The tenant configuration.</param>
		/// <param name="sessionFactory">The session factory.</param>
		/// <returns>The connection string for the tenant.</returns>
		protected abstract string GetTenantConnectionString(TenantConfiguration tenantConfiguration, ISessionFactoryImplementor sessionFactory);

		[Serializable]
		partial class ContextualConnectionAccess : IConnectionAccess
		{
			private readonly ISessionFactoryImplementor _sessionFactory;

			public ContextualConnectionAccess(string connectionString, ISessionFactoryImplementor sessionFactory)
			{
				ConnectionString = connectionString;
				_sessionFactory = sessionFactory;
			}

			/// <inheritdoc />
			public string ConnectionString { get; }

			/// <inheritdoc />
			public DbConnection GetConnection()
			{
				return _sessionFactory.ConnectionProvider.GetConnection(ConnectionString);
			}

			/// <inheritdoc />
			public void CloseConnection(DbConnection connection)
			{
				_sessionFactory.ConnectionProvider.CloseConnection(connection);
			}
		}
	}
}
