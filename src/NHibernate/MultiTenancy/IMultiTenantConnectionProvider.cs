using NHibernate.Connection;

namespace NHibernate.MultiTenancy
{
	/// <summary>
	/// A specialized Connection provider contract used when the application is using multi-tenancy support requiring
	/// tenant aware connections.
	/// </summary>
	public interface IMultiTenantConnectionProvider
	{
		/// <summary>
		/// Tenant identifier must uniquely identify tenant (is used for data separation in cache)
		/// Note: Among other things this value is used for data separation between tenants in cache so not unique value will leak data to other tenants 
		/// </summary>
		string TenantIdentifier { get; } 

		/// <summary>
		/// Tenant connection access
		/// </summary>
		IConnectionAccess GetConnectionAccess();
	}
}
