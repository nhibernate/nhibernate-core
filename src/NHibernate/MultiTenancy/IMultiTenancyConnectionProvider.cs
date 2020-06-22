using NHibernate.Connection;

namespace NHibernate.MultiTenancy
{
	/// <summary>
	/// A specialized Connection provider contract used when the application is using multi-tenancy support requiring
	/// tenant aware connections.
	/// </summary>
	public interface IMultiTenancyConnectionProvider
	{
		/// <summary>
		/// Tenant connection access
		/// </summary>
		IConnectionAccess GetConnectionAccess(TenantConfiguration configuration);
	}
}
