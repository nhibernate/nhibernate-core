using System;

namespace NHibernate.MultiTenancy
{
	/// <summary>
	/// Tenant specific configuration
	/// </summary>
	[Serializable]
	public class TenantConfiguration
	{
		/// <summary>
		/// Tenant identifier must uniquely identify tenant
		/// Note: Among other things this value is used for data separation between tenants in cache so not unique value will leak data to other tenants 
		/// </summary>
		public string TenantIdentifier { get; }

		public TenantConfiguration(string tenantIdentifier)
		{
			TenantIdentifier = tenantIdentifier ?? throw new ArgumentNullException(nameof(tenantIdentifier));
		}
	}
}
