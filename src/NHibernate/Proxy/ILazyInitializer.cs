using NHibernate.Engine;

namespace NHibernate.Proxy
{
	public partial interface ILazyInitializer
	{
		/// <summary>
		/// Perform an ImmediateLoad of the actual object for the Proxy.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the Proxy has no Session or the Session is closed or disconnected.
		/// </exception>
		void Initialize();

		/// <summary>
		/// The identifier value for the entity our owning proxy represents.
		/// </summary>
		object Identifier { get; set; }

		/// <summary>
		/// The entity-name of the entity our owning proxy represents.
		/// </summary>
		string EntityName { get;}

		/// <summary>
		/// Get the actual class of the entity.  Generally, <see cref="EntityName" /> should be used instead.
		/// </summary>
		System.Type PersistentClass { get; }

		/// <summary>
		/// Is the proxy uninitialized?
		/// </summary>
		bool IsUninitialized { get; }

		bool Unwrap { get; set; }

		/// <summary>
		/// Get the session to which this proxy is associated, or null if it is not attached.
		/// </summary>
		ISessionImplementor Session { get; set; }
		
		/// <summary>
		/// Is the read-only setting available?
		/// </summary>
		bool IsReadOnlySettingAvailable { get; }
		
		/// <summary>
		/// Read-only status
		/// </summary>
		/// <remarks>
		/// <para>
		/// Not available when the proxy is detached or its associated session is closed.
		/// </para>
		/// <para>
		/// To check if the read-only setting is available, use <see cref="IsReadOnlySettingAvailable" />
	 	/// </para>
		/// <para>
		/// The read-only status of the entity will be made to match the read-only status of the proxy
		/// upon initialization.
		/// </para>
		/// </remarks>
		bool ReadOnly { get; set; }

		/// <summary>
		/// Return the underlying persistent object, initializing if necessary.
		/// </summary>
		/// <returns>The persistent object this proxy is proxying.</returns>
		object GetImplementation();

		/// <summary>
		/// Return the underlying persistent object in a given <see cref="ISession"/>, or null.
		/// </summary>
		/// <param name="s">The session to get the object from.</param>
		/// <returns>The persistent object this proxy is proxying, or <see langword="null" />.</returns>
		object GetImplementation(ISessionImplementor s);

		/// <summary>
		/// Initialize the proxy manually by injecting its target.
		/// </summary>
		/// <param name="target">The proxy target (the actual entity being proxied).</param>
		void SetImplementation(object target);

		/// <summary>
		/// Associate the proxy with the given session.
		///
		/// Care should be given to make certain that the proxy is added to the session's persistence context as well
		/// to maintain the symmetry of the association.  That must be done separately as this method simply sets an
		/// internal reference.  We do also check that if there is already an associated session that the proxy
		/// reference was removed from that previous session's persistence context.
		/// </summary>
		/// <param name="s">The session</param>
		void SetSession(ISessionImplementor s);
		
		/// <summary>
		/// Unset this initializer's reference to session.  It is assumed that the caller is also taking care or
		/// cleaning up the owning proxy's reference in the persistence context.
		///
		/// Generally speaking this is intended to be called only during <see cref="NHibernate.ISession.Evict" /> and
		/// <see cref="NHibernate.ISession.Clear()" /> processing; most other use-cases should call <see cref="SetSession(ISessionImplementor)" /> instead.
		/// </summary>
		void UnsetSession();
	}
}