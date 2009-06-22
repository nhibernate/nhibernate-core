using System;
using log4net;

namespace NHibernate.Impl
{
	using System.Web;

	public class SessionIdLoggingContext : IDisposable
	{
		[ThreadStatic] private static Guid? CurrentSessionId;

		private const string CurrentSessionIdKey = "NHibernate.Impl.SessionIdLoggingContext.CurrentSessionId";

		private readonly Guid? oldSessonId;

		public SessionIdLoggingContext(Guid id)
		{
			oldSessonId = SessionId;
			SessionId = id;
		}

		/// <summary>
		/// Error handling in this case will only kick in if we cannot set values on the TLS
		/// this is usally the case if we are called from the finalizer, since this is something
		/// that we do only for logging, we ignore the error.
		/// </summary>
		public static Guid? SessionId
		{
			get
			{
				if (HttpContext.Current != null)
					return (Guid?)HttpContext.Current.Items[CurrentSessionIdKey];
				return CurrentSessionId;
			}
			set
			{
				if (HttpContext.Current != null)
					HttpContext.Current.Items[CurrentSessionIdKey] = value;
				else
					CurrentSessionId = value;
			}
		}

		public void Dispose()
		{
			SessionId = oldSessonId;
		}
	}
}