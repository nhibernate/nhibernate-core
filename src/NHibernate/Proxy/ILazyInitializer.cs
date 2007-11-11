using NHibernate.Engine;

namespace NHibernate.Proxy
{
	public interface ILazyInitializer
	{
		/// <summary>
		/// Perform an ImmediateLoad of the actual object for the Proxy.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the Proxy has no Session or the Session is closed or disconnected.
		/// </exception>
		void Initialize();

		/// <summary></summary>
		object Identifier { get; set; }

		/// <summary></summary>
		System.Type PersistentClass { get; }

		/// <summary></summary>
		bool IsUninitialized { get; }

		bool Unwrap { get; set; }

		/// <summary></summary>
		ISessionImplementor Session { get; set; }

		/// <summary>
		/// Return the Underlying Persistent Object, initializing if necessary.
		/// </summary>
		/// <returns>The Persistent Object this proxy is Proxying.</returns>
		object GetImplementation();

		/// <summary>
		/// Return the Underlying Persistent Object in a given <see cref="ISession"/>, or null.
		/// </summary>
		/// <param name="s">The Session to get the object from.</param>
		/// <returns>The Persistent Object this proxy is Proxying, or <see langword="null" />.</returns>
		object GetImplementation(ISessionImplementor s);

		void SetImplementation(object target);
	}
}