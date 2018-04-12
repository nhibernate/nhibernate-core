using System.Data.Common;
using NHibernate.Connection;

namespace NHibernate
{
	// NH different implementation: will not try to support covariant return type for specializations
	// until it is needed.
	/// <summary>
	/// Specialized <see cref="ISessionBuilder"/> with access to stuff from another session.
	/// </summary>
	// 6.0 TODO: implement covariance the way used for ISharedSessionBuilder
	public interface ISharedStatelessSessionBuilder : IStatelessSessionBuilder
	{
		#region 6.0 TODO: implement covariance the way used for ISharedSessionBuilder

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
		new ISharedStatelessSessionBuilder Connection(DbConnection connection);

		/// <summary>
		/// Should the session be automatically enlisted in ambient system transaction?
		/// Enabled by default. Disabling it does not prevent connections having auto-enlistment
		/// enabled to get enlisted in current ambient transaction when opened.
		/// </summary>
		/// <param name="autoJoinTransaction">Should the session be automatically explicitly
		/// enlisted in ambient transaction.</param>
		/// <returns><see langword="this" />, for method chaining.</returns>
		new ISharedStatelessSessionBuilder AutoJoinTransaction(bool autoJoinTransaction);

		#endregion

		/// <summary>
		/// Signifies that the connection from the original session should be used to create the new session.
		/// The original session remains responsible for it and its closing will cause sharing sessions to be no
		/// more usable.
		/// Causes specified <c>ConnectionReleaseMode</c> and <c>AutoJoinTransaction</c> to be ignored and
		/// replaced by those of the original session.
		/// </summary>
		/// <returns><see langword="this" />, for method chaining.</returns>
		ISharedStatelessSessionBuilder Connection();

		/// <summary>
		/// Signifies that the AutoJoinTransaction flag from the original session should be used to create the new session.
		/// </summary>
		/// <returns><see langword="this" />, for method chaining.</returns>
		ISharedStatelessSessionBuilder AutoJoinTransaction();
	}
}
