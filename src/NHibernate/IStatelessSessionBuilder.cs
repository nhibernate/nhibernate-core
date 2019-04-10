using System.Data.Common;
using NHibernate.Connection;
using NHibernate.Impl;
using NHibernate.MultiTenancy;
using NHibernate.Util;

namespace NHibernate
{
	//TODO 6.0: Merge into IStatelessSessionBuilder
	public static class StatelessSessionBuilderExtensions
	{
		/// <summary>
		/// Provides tenant configuration required for multi-tenancy
		/// <seealso cref="Cfg.Environment.MultiTenancy"/>
		/// </summary>
		public static T TenantConfiguration<T>(this T builder, TenantConfiguration tenantConfig) where T: IStatelessSessionBuilder
		{
			ReflectHelper.CastOrThrow<ISessionCreationOptionsWithMultiTenancy>(builder, "multi tenancy").TenantConfiguration = tenantConfig;
			return builder;
		}
	}

	// NH different implementation: will not try to support covariant return type for specializations
	// until it is needed.
	/// <summary>
	/// Represents a consolidation of all stateless session creation options into a builder style delegate.
	/// </summary>
	public interface IStatelessSessionBuilder
	{
		/// <summary>
		/// Opens a session with the specified options.
		/// </summary>
		/// <returns>The session.</returns>
		IStatelessSession OpenStatelessSession();

		/// <summary>
		/// Adds a specific connection to the session options.
		/// </summary>
		/// <param name="connection">The connection to use.</param>
		/// <returns><see langword="this" />, for method chaining.</returns>
		/// <remarks>
		/// Note that the second-level cache will be disabled if you
		/// supply a ADO.NET connection. NHibernate will not be able to track
		/// any statements you might have executed in the same transaction.
		/// Consider implementing your own <see cref="IConnectionProvider" />.
		/// </remarks>
		IStatelessSessionBuilder Connection(DbConnection connection);

		/// <summary>
		/// Should the session be automatically enlisted in ambient system transaction?
		/// Enabled by default. Disabling it does not prevent connections having auto-enlistment
		/// enabled to get enlisted in current ambient transaction when opened.
		/// </summary>
		/// <param name="autoJoinTransaction">Should the session be automatically explicitly
		/// enlisted in ambient transaction.</param>
		/// <returns><see langword="this" />, for method chaining.</returns>
		IStatelessSessionBuilder AutoJoinTransaction(bool autoJoinTransaction);
	}
}
