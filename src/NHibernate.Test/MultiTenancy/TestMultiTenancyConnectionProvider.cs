using System;
using System.Data.SqlClient;
using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.MultiTenancy;

namespace NHibernate.Test.MultiTenancy
{
	[Serializable]
	public class TestMultiTenancyConnectionProvider : AbstractMultiTenancyConnectionProvider
	{
		protected override string GetTenantConnectionString(TenantConfiguration tenantConfiguration, ISessionFactoryImplementor sessionFactory)
		{
			return tenantConfiguration is TestTenantConfiguration tenant && tenant.IsSqlServerDialect
				? new SqlConnectionStringBuilder(sessionFactory.ConnectionProvider.GetConnectionString()) { ApplicationName = tenantConfiguration.TenantIdentifier }.ToString()
				: sessionFactory.ConnectionProvider.GetConnectionString();
		}
	}
}
