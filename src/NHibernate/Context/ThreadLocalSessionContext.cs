using System;
using System.Collections.Generic;

using NHibernate;
using NHibernate.Engine;

namespace NHibernate.Context
{
	//TODO: refactoring on this class. Maybe using MapBasedSessionContext.
	/// <summary>
	/// A <see cref="ICurrentSessionContext"/> impl which scopes the notion of current
	/// session by the current thread of execution. Threads do not give us a 
	/// nice hook to perform any type of cleanup making
	/// it questionable for this impl to actually generate Session instances.  In
	/// the interest of usability, it was decided to have this default impl
	/// actually generate a session upon first request and then clean it up
	/// after the <see cref="ITransaction"/> associated with that session
	/// is committed/rolled-back.  In order for ensuring that happens, the sessions
	/// generated here are unusable until after {@link Session#beginTransaction()}
	/// has been called. If <tt>Close()</tt> is called on a session managed by
	/// this class, it will be automatically unbound.
	/// <p/>
	/// Additionally, the static <see cref="Bind"/> and <see cref="Unbind"/> methods are
	/// provided to allow application code to explicitly control opening and
	/// closing of these sessions.  This, with some from of interception,
	/// is the preferred approach.  It also allows easy framework integration
	/// and one possible approach for implementing long-sessions.
	/// <p/>
	/// </summary>
	[Serializable]
	public class ThreadLocalSessionContext : ICurrentSessionContext
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ThreadLocalSessionContext));

		[ThreadStatic]
		protected static IDictionary<ISessionFactory, ISession> context;

		protected readonly ISessionFactoryImplementor factory;


		public ThreadLocalSessionContext(ISessionFactoryImplementor factory)
		{
			this.factory = factory;
		}

		#region ICurrentSessionContext Members

		public ISession CurrentSession()
		{
			ISession current = ExistingSession(factory);
			if (current == null)
			{
				current = BuildOrObtainSession();

				// wrap the session in the transaction-protection proxy
				if (NeedsWrapping(current))
				{
					current = Wrap(current);
				}
				// then bind it
				DoBind(current, factory);
			}
			return current;
		}

		#endregion

		private static void CleanupAnyOrphanedSession(ISessionFactory factory)
		{
			ISession orphan = DoUnbind(factory, false);

			if (orphan != null)
			{
				log.Warn("Already session bound on call to bind(); make sure you clean up your sessions!");

				try
				{
					if (orphan.Transaction != null && orphan.Transaction.IsActive)
					{
						try
						{
							orphan.Transaction.Rollback();
						}
						catch (Exception ex)
						{
							log.Debug("Unable to rollback transaction for orphaned session", ex);
						}
					}
					orphan.Close();
				}
				catch (Exception ex)
				{
					log.Debug("Unable to close orphaned session", ex);
				}
			}
		}

		public static void Bind(ISession session)
		{
			ISessionFactory factory = session.SessionFactory;
			CleanupAnyOrphanedSession(factory);
			DoBind(session, factory);
		}

		/// <summary>
		/// Unassociate a previously bound session from the current thread of execution.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public static ISession Unbind(ISessionFactory factory)
		{
			return DoUnbind(factory, true);
		}

		private static void DoBind(ISession current, ISessionFactory factory)
		{
			context = context ?? new Dictionary<ISessionFactory, ISession>();

			context.Add(factory, current);
		}

		private static ISession DoUnbind(ISessionFactory factory, bool releaseMapIfEmpty)
		{
			ISession session = null;

			if (context != null)
			{
				if (context.TryGetValue(factory, out session))
				{
					context.Remove(factory);
				}

				if (releaseMapIfEmpty && context.Count == 0)
					context = null;
			}
			return session;
		}

		private ISession Wrap(ISession current)
		{
			return current;
		}

		private bool NeedsWrapping(ISession current)
		{
			return false;
		}

		protected ISession BuildOrObtainSession()
		{
			return factory.OpenSession(
				null,
				IsAutoFlushEnabled(),
				IsAutoCloseEnabled(),
				GetConnectionReleaseMode());
		}

		private ConnectionReleaseMode GetConnectionReleaseMode()
		{
			return factory.Settings.ConnectionReleaseMode;
		}

		protected virtual bool IsAutoCloseEnabled()
		{
			return true;
		}

		protected virtual bool IsAutoFlushEnabled()
		{
			return true;
		}

		private static ISession ExistingSession(ISessionFactory factory)
		{
			if (context == null)
				return null;
		
			ISession result;
			context.TryGetValue(factory, out result);
			return result;
		}
	}
}