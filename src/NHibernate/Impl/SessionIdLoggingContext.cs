using System;
using log4net;

namespace NHibernate.Impl
{
	public class SessionIdLoggingContext : IDisposable
	{
		private readonly object oldSessonId;

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
		private static object SessionId
		{
			get
			{
				try
				{
					return ThreadContext.Properties["sessionId"];
				}
				catch (Exception)
				{
					return null;
				}
			}
			set
			{
				try
				{
					ThreadContext.Properties["sessionId"] = value;
				}
				catch (Exception)
				{
				}
			}
		}

		public void Dispose()
		{
			SessionId = oldSessonId;
		}
	}
}