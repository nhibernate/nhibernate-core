using System;
#if NETFX
using System.Runtime.Remoting.Messaging;
#else
using System.Threading;
#endif

namespace NHibernate.Impl
{
	public class SessionIdLoggingContext : IDisposable
	{
#if !NETFX
		private static readonly Lazy<AsyncLocal<Guid?>> _currentSessionId =
			new Lazy<AsyncLocal<Guid?>>(() => new AsyncLocal<Guid?>(), true);
#else
		private const string LogicalCallContextVariableName = "__" + nameof(SessionIdLoggingContext) + "__";
#endif
		private readonly Guid? _oldSessonId;
		private readonly bool _hasChanged;
		
		public SessionIdLoggingContext(Guid id)
		{
			if (id == Guid.Empty) return;
			_oldSessonId = SessionId;
			if (id == _oldSessonId) return;
			_hasChanged = true;
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
			get
			{
#if !NETFX
				return _currentSessionId.IsValueCreated ? _currentSessionId.Value.Value : null;
#else
				return (Guid?) CallContext.LogicalGetData(LogicalCallContextVariableName);
#endif
			}
			set
			{
#if !NETFX
				_currentSessionId.Value.Value = value;
#else
				CallContext.LogicalSetData(LogicalCallContextVariableName, value);
#endif
			}
		}

		public void Dispose()
		{
			if (_hasChanged)
			{
				SessionId = _oldSessonId;
			}
		}
	}
}
