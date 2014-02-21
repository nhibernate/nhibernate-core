using System;
using System.Collections.Generic;
using System.Threading;

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
	/// </summary>
	[Serializable]
	public class ThreadLocalSessionContext : CurrentSessionContext
	{
		private ThreadLocal<ISession> _session = new ThreadLocal<ISession>();

		public ThreadLocalSessionContext(Engine.ISessionFactoryImplementor factory)
		{
		}

		/// <summary> Gets or sets the currently bound session. </summary>
		protected override ISession Session
		{
			get { return _session.Value; }
			set { _session.Value = value; }
		}
	}
}