using NHibernate.Bytecode;
using NHibernate.Engine;

namespace NHibernate.Context
{
	/// <summary>
	/// Defines the contract for implementations which know how to
	/// scope the notion of a <see cref="ISessionFactory.GetCurrentSession()">current session</see>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Implementations should adhere to the following:
	/// <list type="bullet">
	/// <item><description>contain a constructor accepting a single argument of type
	/// <see cref="ISessionFactoryImplementor" />, or implement
	/// <see cref="ISessionFactoryAwareCurrentSessionContext"/></description></item>
	/// <item><description>should be thread safe</description></item>
	/// <item><description>should be fully serializable</description></item>
	/// </list>
	/// </para>
	/// <para>
	/// Implementors should be aware that they are also fully responsible for
	/// cleanup of any generated current-sessions.
	/// </para>
	/// <para>
	/// Note that there will be exactly one instance of the configured
	/// ICurrentSessionContext implementation per <see cref="ISessionFactory" />.
	/// </para>
	/// <para>
	/// It is recommended to inherit from the class <see cref="CurrentSessionContext" />
	/// whenever possible as it simplifies the implementation and provides
	/// single entry point with session binding support.
	/// </para>
	/// </remarks>
	public interface ICurrentSessionContext
	{
		/// <summary>
		/// Retrieve the current session according to the scoping defined
		/// by this implementation.
		/// </summary>
		/// <returns>The current session.</returns>
		/// <exception cref="HibernateException">Typically indicates an issue
		/// locating or creating the current session.</exception>
		ISession CurrentSession();
	}

	/// <summary>
	/// An <see cref="ICurrentSessionContext"/> allowing to set its session factory. Implementing
	/// this interface allows the <see cref="IObjectsFactory"/> to be used for instantiating the
	/// session context.
	/// </summary>
	public interface ISessionFactoryAwareCurrentSessionContext : ICurrentSessionContext
	{
		/// <summary>
		/// Sets the factory. This method should be called once after creating the context.
		/// </summary>
		/// <param name="factory">The factory.</param>
		void SetFactory(ISessionFactoryImplementor factory);
	}
}
