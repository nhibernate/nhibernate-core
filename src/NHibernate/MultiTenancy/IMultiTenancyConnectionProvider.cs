using NHibernate.Connection;
using NHibernate.Engine;

namespace NHibernate.MultiTenancy
{
	/// <summary>
	/// A specialized Connection provider contract used when the application is using multi-tenancy support requiring
	/// tenant aware connections.
	/// </summary>
	public interface IMultiTenancyConnectionProvider
	{
		/// <summary>
		/// Gets the tenant connection access.
		/// </summary>
		/// <param name="tenantConfiguration">The tenant configuration.</param>
		/// <param name="sessionFactory">The session factory.</param>
		/// <returns>The tenant connection access.</returns>
		IConnectionAccess GetConnectionAccess(TenantConfiguration tenantConfiguration, ISessionFactoryImplementor sessionFactory);
	}
}
