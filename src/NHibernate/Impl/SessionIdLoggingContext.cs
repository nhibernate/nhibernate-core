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
#if NETFX
		private const string LogicalCallContextVariableName = "__" + nameof(SessionIdLoggingContext) + "__";
#else
		private static readonly Lazy<AsyncLocal<Guid?>> _currentSessionId =
			new Lazy<AsyncLocal<Guid?>>(() => new AsyncLocal<Guid?>(), true);
#endif
		private readonly Guid? _oldSessionId;
		private bool _hasChanged;

		[Obsolete("Please use SessionIdLoggingContext.CreateOrNull instead.")]
		public SessionIdLoggingContext(Guid id)
		{
			if (id == Guid.Empty) return;
			_oldSessionId = SessionId;
			if (id == _oldSessionId) return;
			_hasChanged = true;
			SessionId = id;
		}

		private SessionIdLoggingContext(Guid newId, Guid? oldId)
		{
			SessionId = newId;
			_oldSessionId = oldId;
			_hasChanged = true;
		}

		public static IDisposable CreateOrNull(Guid id)
		{
			if (id == Guid.Empty)
				return null;
			var oldId = SessionId;

			if (oldId == id)
				return null;

			return new SessionIdLoggingContext(id, oldId);
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
#if NETFX
				return (Guid?) CallContext.LogicalGetData(LogicalCallContextVariableName);
#else
				return _currentSessionId.IsValueCreated ? _currentSessionId.Value.Value : null;
#endif
			}
			set
			{
#if NETFX
				CallContext.LogicalSetData(LogicalCallContextVariableName, value);
#else
				_currentSessionId.Value.Value = value;
#endif
			}
		}

		public void Dispose()
		{
			if (_hasChanged)
			{
				SessionId = _oldSessionId;
				_hasChanged = false;
			}
		}
	}
}
