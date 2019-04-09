using NHibernate.Connection;

namespace NHibernate.MultiTenancy
{
	/// <summary>
	/// Tenant specific configuration
	/// </summary>
	public class TenantConfiguration
	{
		/// <summary>
		/// Tenant identifier must uniquely identify tenant
		/// Note: Among other things this value is used for data separation between tenants in cache so not unique value will leak data to other tenants 
		/// </summary>
		public string TenantIdentifier { get; set; }

		/// <summary>
		/// Tenant connection access. Required for <seealso cref="MultiTenancyStrategy.Database"/> multi-tenancy strategy.
		/// </summary>
		public IConnectionAccess ConnectionAccess { get; set; }

		/// <summary>
		/// Creates tenant configuration for <seealso cref="MultiTenancyStrategy.Database"/> multi-tenancy strategy.
		/// </summary>
		public TenantConfiguration(IMultiTenantConnectionProvider tenantConnectionProvider)
		{
			TenantIdentifier = tenantConnectionProvider.TenantIdentifier;
			ConnectionAccess = tenantConnectionProvider.GetConnectionAccess();
		}
	}
}
