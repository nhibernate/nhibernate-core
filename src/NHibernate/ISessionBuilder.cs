using System.Data.Common;
using NHibernate.Connection;
using NHibernate.Impl;
using NHibernate.MultiTenancy;
using NHibernate.Util;

namespace NHibernate
{
	//TODO 6.0: Merge into ISessionBuilder<T>
	public static class SessionBuilderExtensions
	{
		/// <summary>
		/// Provides tenant configuration required for multi-tenancy
		/// <seealso cref="Cfg.Environment.MultiTenancy"/>
		/// </summary>
		public static T TenantConfiguration<T>(this T builder, TenantConfiguration tenantConfig) where T: ISessionBuilder
		{
			ReflectHelper.CastOrThrow<ISessionCreationOptionsWithMultiTenancy>(builder, "multi tenancy").TenantConfiguration = tenantConfig;
			return builder;
		}
	}

	// NH specific: Java does not require this, it looks as still having a better covariance support.
	/// <summary>
	/// Represents a consolidation of all session creation options into a builder style delegate.
	/// </summary>
	public interface ISessionBuilder : ISessionBuilder<ISessionBuilder> { }

	//TODO 6.0: Make T covariant ISessionBuilder<T> -> ISessionBuilder<out T> 
	/// <summary>
	/// Represents a consolidation of all session creation options into a builder style delegate.
	/// </summary>
	public interface ISessionBuilder<T> where T : ISessionBuilder<T>
	{
		/// <summary>
		/// Opens a session with the specified options.
		/// </summary>
		/// <returns>The session.</returns>
		ISession OpenSession();

		/// <summary>
		/// Adds a specific interceptor to the session options.
		/// </summary>
		/// <param name="interceptor">The interceptor to use.</param>
		/// <returns><see langword="this" />, for method chaining.</returns>
		T Interceptor(IInterceptor interceptor);

		/// <summary>
		/// Signifies that no <see cref="IInterceptor"/> should be used.
		/// </summary>
		/// <returns><see langword="this" />, for method chaining.</returns>
		/// <remarks>
		/// By default the <see cref="IInterceptor"/> associated with the <see cref="ISessionFactory"/> is
		/// passed to the <see cref="ISession"/> whenever we open one without the user having specified a
		/// specific interceptor to use.
		/// </remarks>
		T NoInterceptor();

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
		T Connection(DbConnection connection);

		/// <summary>
		/// Use a specific connection release mode for these session options.
		/// </summary>
		/// <param name="connectionReleaseMode">The connection release mode to use.</param>
		/// <returns><see langword="this" />, for method chaining.</returns>
		T ConnectionReleaseMode(ConnectionReleaseMode connectionReleaseMode);

		/// <summary>
		/// Should the session be automatically closed after transaction completion? Not yet implemented, will have no effect.
		/// </summary>
		/// <param name="autoClose">Should the session be automatically closed.</param>
		/// <returns><see langword="this" />, for method chaining.</returns>
		T AutoClose(bool autoClose);

		/// <summary>
		/// Should the session be automatically enlisted in ambient system transaction?
		/// Enabled by default. Disabling it does not prevent connections having auto-enlistment
		/// enabled to get enlisted in current ambient transaction when opened.
		/// </summary>
		/// <param name="autoJoinTransaction">Should the session be automatically explicitly
		/// enlisted in ambient transaction.</param>
		/// <returns><see langword="this" />, for method chaining.</returns>
		T AutoJoinTransaction(bool autoJoinTransaction);

		/// <summary>
		/// Specify the initial FlushMode to use for the opened Session.
		/// </summary>
		/// <param name="flushMode">The initial FlushMode to use for the opened Session.</param>
		/// <returns><see langword="this" />, for method chaining.</returns>
		T FlushMode(FlushMode flushMode);
	}
}
