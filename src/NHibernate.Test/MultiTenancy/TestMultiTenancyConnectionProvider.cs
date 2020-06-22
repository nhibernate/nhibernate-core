using System;
using System.Data.SqlClient;
using NHibernate.MultiTenancy;

namespace NHibernate.Test.MultiTenancy
{
	[Serializable]
	public class TestMultiTenancyConnectionProvider : DefaultMultiTenancyConnectionProvider
	{
		protected override string GetTenantConnectionString(TenantConfiguration configuration)
		{
			return configuration is TestTenantConfiguration tenant && tenant.IsSqlServerDialect
				? new SqlConnectionStringBuilder(configuration.ConnectionString) {ApplicationName = configuration.TenantIdentifier}.ToString()
				: base.GetTenantConnectionString(configuration);
		}
	}
}
