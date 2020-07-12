using System;
using NHibernate.MultiTenancy;

namespace NHibernate.Test.MultiTenancy
{
	[Serializable]
	public class TestTenantConfiguration : TenantConfiguration
	{
		public TestTenantConfiguration(string tenantIdentifier, bool isSqlServerDialect) : base(tenantIdentifier)
		{
			IsSqlServerDialect = isSqlServerDialect;
		}

		public bool IsSqlServerDialect { get; }
	}
}
