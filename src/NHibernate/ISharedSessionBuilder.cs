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
		/// Signifies that the autoClose flag from the original session should be used to create the new session.
		/// </summary>
		/// <returns><see langword="this" />, for method chaining.</returns>
		ISharedSessionBuilder AutoClose();
	}
}