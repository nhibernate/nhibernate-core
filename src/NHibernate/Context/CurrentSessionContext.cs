using System;

using NHibernate.Engine;

namespace NHibernate.Context
{
	/// <summary>
	/// Extends the contract defined by <see cref="ICurrentSessionContext"/>
	/// by providing methods to bind and unbind sessions to the current context.
	/// </summary>
	/// <remarks>
	/// The notion of a contextual session is managed by some external entity
	/// (generally some form of interceptor like the HttpModule).
	/// This external manager is responsible for scoping these contextual sessions
	/// appropriately binding/unbinding them here for exposure to the application
	/// through <see cref="ISessionFactory.GetCurrentSession()"/> calls.
	/// </remarks>
	[Serializable]
	public abstract class CurrentSessionContext : ICurrentSessionContext
	{
		/// <summary> Gets or sets the currently bound session. </summary>
		protected abstract ISession Session { get; set; }

		/// <summary>
		/// Retrieve the current session according to the scoping defined
		/// by this implementation.
		/// </summary>
		/// <returns>The current session.</returns>
		/// <exception cref="HibernateException">Indicates an issue
		/// locating the current session.</exception>
		public virtual ISession CurrentSession()
		{
			if (Session == null)
			{
				throw new HibernateException("No session bound to the current context");
			}
			return Session;
		}

		/// <summary>
		/// Binds the specified session to the current context.
		/// </summary>
		public static void Bind(ISession session)
		{
			GetCurrentSessionContext(session.SessionFactory).Session = session;
		}

		/// <summary>
		/// Returns whether there is a session bound to the current context.
		/// </summary>
		public static bool HasBind(ISessionFactory factory)
		{
			return GetCurrentSessionContext(factory).Session != null;
		}

		/// <summary>
		/// Unbinds and returns the current session.
		/// </summary>
		public static ISession Unbind(ISessionFactory factory)
		{
			ISession removedSession = GetCurrentSessionContext(factory).Session;
			GetCurrentSessionContext(factory).Session = null;
			return removedSession;
		}

		private static CurrentSessionContext GetCurrentSessionContext(ISessionFactory factory)
		{
			ISessionFactoryImplementor factoryImpl = factory as ISessionFactoryImplementor;

			if (factoryImpl == null)
			{
				throw new HibernateException("Session factory does not implement ISessionFactoryImplementor.");
			}
			
			if (factoryImpl.CurrentSessionContext == null)
			{
				throw new HibernateException("No current session context configured.");
			}
			
			CurrentSessionContext currentSessionContext = factoryImpl.CurrentSessionContext as CurrentSessionContext;
			if (currentSessionContext == null)
			{
				throw new HibernateException("Current session context does not extend class CurrentSessionContext.");
			}
			
			return currentSessionContext;
		}
	}
}