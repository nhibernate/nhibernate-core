using System;
using System.Collections.Generic;
using System.Threading;
using NHibernate;
using NHibernate.Engine;

namespace NHibernate.Context
{
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
	/// Additionally, the static <see cref="CurrentSessionContext.Bind"/> and <see cref="CurrentSessionContext.Unbind"/> methods are
	/// provided to allow application code to explicitly control opening and
	/// closing of these sessions.  This, with some from of interception,
	/// is the preferred approach.  It also allows easy framework integration
	/// and one possible approach for implementing long-sessions.
	/// <p/>
	/// </summary>
	[Serializable]
	public class ThreadLocalSessionContext : CurrentSessionContext
	{
		private ThreadLocal<ISession> _session = new ThreadLocal<ISession>();

		public ThreadLocalSessionContext(ISessionFactoryImplementor factory)
		{
		}

		protected override ISession Session
		{
			get { return (this._session.Value); }
			set { this._session.Value = value; }
		}
	}
}