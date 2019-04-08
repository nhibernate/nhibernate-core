using NHibernate.Connection;

namespace NHibernate.MultiTenancy
{
	/// <summary>
	/// Tenant specific configuration
	/// </summary>
	public class TenantConfiguration
	{
		public string TenantIdentifier { get; set; }
		public IConnectionAccess ConnectionAccess { get; set; }

		public TenantConfiguration(IMultiTenantConnectionProvider tenantConnectionProvider)
		{
			TenantIdentifier = tenantConnectionProvider.TenantIdentifier;
			ConnectionAccess = tenantConnectionProvider.GetConnectionAccess();
		}
	}
}
