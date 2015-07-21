using System;

namespace NHibernate.Impl
{
	public class SessionIdLoggingContext : IDisposable
	{
		[ThreadStatic]
		private static Guid? CurrentSessionId;

		private readonly Guid? oldSessonId;

		public SessionIdLoggingContext(Guid id)
		{
			oldSessonId = SessionId;
			SessionId = id;
		}

		/// <summary>
		/// We always set the result to use a thread static variable, on the face of it,
		/// it looks like it is not a valid choice, since ASP.Net and WCF may decide to switch
		/// threads on us. But, since SessionIdLoggingContext is only used inside NH calls, and since
		/// NH calls are never async, this isn't an issue for us.
		/// In addition to that, attempting to match to the current context has proven to be performance hit.
		/// </summary>
		public static Guid? SessionId
		{
			get { return CurrentSessionId; }
			set { CurrentSessionId = value; }
		}

		#region IDisposable Members

		public void Dispose()
		{
			SessionId = oldSessonId;
		}

		#endregion
	}
}