using System;
using System.Threading;

namespace NHibernate.Impl
{
	public class SessionIdLoggingContext : IDisposable
	{
		private static readonly Lazy<AsyncLocal<Guid?>> _currentSessionId =
			new Lazy<AsyncLocal<Guid?>>(() => new AsyncLocal<Guid?>(), true);

		private readonly Guid? _oldSessonId;
		private readonly bool _tracking;

		public SessionIdLoggingContext(Guid id)
		{
			_tracking = id != Guid.Empty;
			if (!_tracking)
			{
				return;
			}
			_oldSessonId = SessionId;
			SessionId = id;
		}

		/// <summary>
		/// We always set the result to use an async local variable, on the face of it,
		/// it looks like it is not a valid choice, since ASP.Net and WCF may decide to switch
		/// threads on us. But, since SessionIdLoggingContext is only used inside NH calls, and since
		/// NH calls are either async-await or fully synchronous, this isn't an issue for us.
		/// In addition to that, attempting to match to the current context has proven to be performance hit.
		/// </summary>
		public static Guid? SessionId
		{
			get => _currentSessionId.IsValueCreated ? _currentSessionId.Value.Value : null;
			set => _currentSessionId.Value.Value = value;
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (!_tracking)
			{
				return;
			}
			SessionId = _oldSessonId;
		}

		#endregion
	}
}