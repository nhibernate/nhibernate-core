namespace NHibernate
{
	// NH different implementation: will not try to support covariant return type for specializations
	// until it is needed.
	/// <summary>
	/// Specialized <see cref="ISessionBuilder"/> with access to stuff from another session.
	/// </summary>
	public interface ISharedSessionBuilder : ISessionBuilder<ISharedSessionBuilder>
	{
		/// <summary>
		/// Signifies that the connection from the original session should be used to create the new session.
		/// The original session remains responsible for it and its closing will cause sharing sessions to be no
		/// more usable.
		/// Causes specified <c>ConnectionReleaseMode</c> and <c>AutoJoinTransaction</c> to be ignored and
		/// replaced by those of the original session.
		/// </summary>
		/// <returns><see langword="this" />, for method chaining.</returns>
		ISharedSessionBuilder Connection();

		/// <summary>
		/// Signifies the interceptor from the original session should be used to create the new session.
		/// </summary>
		/// <returns><see langword="this" />, for method chaining.</returns>
		ISharedSessionBuilder Interceptor();

		/// <summary>
		/// Signifies that the connection release mode from the original session should be used to create the new session.
		/// </summary>
		/// <returns><see langword="this" />, for method chaining.</returns>
		ISharedSessionBuilder ConnectionReleaseMode();

		/// <summary>
		/// Signifies that the FlushMode from the original session should be used to create the new session.
		/// </summary>
		/// <returns><see langword="this" />, for method chaining.</returns>
		ISharedSessionBuilder FlushMode();

		/// <summary>
		/// Signifies that the AutoClose flag from the original session should be used to create the new session.
		/// </summary>
		/// <returns><see langword="this" />, for method chaining.</returns>
		ISharedSessionBuilder AutoClose();

		/// <summary>
		/// Signifies that the AutoJoinTransaction flag from the original session should be used to create the new session.
		/// </summary>
		/// <returns><see langword="this" />, for method chaining.</returns>
		ISharedSessionBuilder AutoJoinTransaction();
	}
}