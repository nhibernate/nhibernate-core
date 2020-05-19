using System;
using System.Data.SqlClient;
using NHibernate.Connection;
using NHibernate.Engine;
using NHibernate.MultiTenancy;

namespace NHibernate.Test.MultiTenancy
{
	[Serializable]
	public class TestTenantConnectionProvider : AbstractMultiTenantConnectionProvider
	{
		public TestTenantConnectionProvider(ISessionFactoryImplementor sfi, string tenantId, bool isSqlServerDialect)
		{
			TenantIdentifier = tenantId;
			SessionFactory = sfi;
			TenantConnectionString = sfi.ConnectionProvider.GetConnectionString();
			if (isSqlServerDialect)
			{
				var stringBuilder = new SqlConnectionStringBuilder(sfi.ConnectionProvider.GetConnectionString());
				stringBuilder.ApplicationName = tenantId;
				TenantConnectionString = stringBuilder.ToString();
			}
		}

		protected override string TenantConnectionString { get; }

		public override string TenantIdentifier { get; }

		protected override ISessionFactoryImplementor SessionFactory { get; }
	}
}
