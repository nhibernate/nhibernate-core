using System;
using System.Data.Common;
using NHibernate.Connection;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.MultiTenancy
{
	/// <summary>
	/// Base implementation for <seealso cref="MultiTenancyStrategy.Database"/> multi-tenancy strategy
	/// </summary>
	[Serializable]
	public partial class DefaultMultiTenancyConnectionProvider : IMultiTenancyConnectionProvider
	{
		/// <inheritdoc />
		public IConnectionAccess GetConnectionAccess(TenantConfiguration configuration)
		{
			var tenantConnectionString = GetTenantConnectionString(configuration);
			if (string.IsNullOrEmpty(tenantConnectionString))
			{
				throw new HibernateException(
					message: $"Tenant '{configuration.TenantIdentifier}' connection string is empty." +
							$" Either provide it on each session opening in TenantConfiguration.ConnectionString or" +
							$" provide custom IMultiTenantConnectionProvider via '{Environment.MultiTenancyConnectionProvider}` session factory setting.");
			}

			return new ContextualConnectionAccess(tenantConnectionString);
		}

		protected virtual string GetTenantConnectionString(TenantConfiguration configuration)
		{
			return configuration.ConnectionString;
		}

		[Serializable]
		partial class ContextualConnectionAccess : IConnectionAccess
		{
			public ContextualConnectionAccess(string connectionString)
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
}
