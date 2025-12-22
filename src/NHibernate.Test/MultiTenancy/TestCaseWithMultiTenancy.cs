using System.Data.Common;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.MultiTenancy;

namespace NHibernate.Test.MultiTenancy
{
	public abstract class TestCaseWithMultiTenancy : TestCase
	{
		protected TestCaseWithMultiTenancy(string tenantIdentifier)
		{
			TenantIdentifier = tenantIdentifier;
		}

		public string TenantIdentifier { get; }

		protected override void Configure(Configuration configuration)
		{
			if (TenantIdentifier != null)
			{
				configuration.DataBaseIntegration(
					x =>
					{
						x.MultiTenancy = MultiTenancyStrategy.Database;
						x.MultiTenancyConnectionProvider<TestMultiTenancyConnectionProvider>();
					});
			}
			base.Configure(configuration);
		}

		protected override DbConnection OpenConnectionForSchemaExport()
		{
			if (TenantIdentifier != null)
			{
				return Sfi.Settings.MultiTenancyConnectionProvider
					.GetConnectionAccess(GetTenantConfig("defaultTenant"), Sfi).GetConnection();
			}
			return base.OpenConnectionForSchemaExport();
		}

		private TenantConfiguration GetTenantConfig(string tenantId)
		{
			return new TestTenantConfiguration(tenantId, IsSqlServerDialect);
		}

		protected override ISession OpenSession()
		{
			if (TenantIdentifier != null)
			{
				return Sfi.WithOptions().Tenant(new TestTenantConfiguration(TenantIdentifier, IsSqlServerDialect)).OpenSession();
			}
			return base.OpenSession();
		}

		//Create extension method for this?
		private bool IsSqlServerDialect => Sfi.Dialect is MsSql2000Dialect && !(Sfi.ConnectionProvider.Driver is OdbcDriver);

	}
}
