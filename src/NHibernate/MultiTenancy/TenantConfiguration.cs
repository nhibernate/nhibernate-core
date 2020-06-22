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
		public string TenantIdentifier { get; set; }

		/// <summary>
		/// Tenant Connection String. Usage depends on multi-tenancy connection provider "multiTenancy.connection.provider"
		/// </summary>
		public string ConnectionString { get; set; }

		public TenantConfiguration(string tenantIdentifier)
		{
			TenantIdentifier = tenantIdentifier;
		}
	}
}
