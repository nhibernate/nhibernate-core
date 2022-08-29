using System;

namespace NHibernate
{
	// Since v5.1
	[Obsolete("Implement and use NHibernate.INHibernateLogger")]
	public interface IInternalLogger
	{
		// Since v5.1
		[Obsolete("Please use IsErrorEnabled() INHibernateLogger extension method instead.")]
		bool IsErrorEnabled { get; }
		// Since v5.1
		[Obsolete("Please use IsFatalEnabled() INHibernateLogger extension method instead.")]
		bool IsFatalEnabled { get; }
		// Since v5.1
		[Obsolete("Please use IsDebugEnabled() INHibernateLogger extension method instead.")]
		bool IsDebugEnabled { get; }
		// Since v5.1
		[Obsolete("Please use IsInfoEnabled() INHibernateLogger extension method instead.")]
		bool IsInfoEnabled { get; }
		// Since v5.1
		[Obsolete("Please use IsWarnEnabled() INHibernateLogger extension method instead.")]
		bool IsWarnEnabled { get; }

		// Since v5.1
		[Obsolete("Please use Error(string, params object[]) INHibernateLogger extension method instead.")]
		void Error(object message);
		// Since v5.1
		[Obsolete("Please use Error(Exception, string, params object[]) INHibernateLogger extension method instead.")]
		void Error(object message, Exception exception);
		// Since v5.1
		[Obsolete("Please use Error(string, params object[]) INHibernateLogger extension method instead.")]
		void ErrorFormat(string format, params object[] args);

		// Since v5.1
		[Obsolete("Please use Fatal(string, params object[]) INHibernateLogger extension method instead.")]
		void Fatal(object message);
		// Since v5.1
		[Obsolete("Please use Fatal(Exception, string, params object[]) INHibernateLogger extension method instead.")]
		void Fatal(object message, Exception exception);

		// Since v5.1
		[Obsolete("Please use Debug(string, params object[]) INHibernateLogger extension method instead.")]
		void Debug(object message);
		// Since v5.1
		[Obsolete("Please use Debug(Exception, string, params object[]) INHibernateLogger extension method instead.")]
		void Debug(object message, Exception exception);
		// Since v5.1
		[Obsolete("Please use Debug(string, params object[]) INHibernateLogger extension method instead.")]
		void DebugFormat(string format, params object[] args);

		// Since v5.1
		[Obsolete("Please use Info(string, params object[]) INHibernateLogger extension method instead.")]
		void Info(object message);
		// Since v5.1
		[Obsolete("Please use Info(Exception, string, params object[]) INHibernateLogger extension method instead.")]
		void Info(object message, Exception exception);
		// Since v5.1
		[Obsolete("Please use Info(string, params object[]) INHibernateLogger extension method instead.")]
		void InfoFormat(string format, params object[] args);

		// Since v5.1
		[Obsolete("Please use Warn(string, params object[]) INHibernateLogger extension method instead.")]
		void Warn(object message);
		// Since v5.1
		[Obsolete("Please use Warn(Exception, string, params object[]) INHibernateLogger extension method instead.")]
		void Warn(object message, Exception exception);
		// Since v5.1
		[Obsolete("Please use Warn(string, params object[]) INHibernateLogger extension method instead.")]
		void WarnFormat(string format, params object[] args);
	}

	// Since 5.1
	[Obsolete("Implement INHibernateLoggerFactory instead")]
	public interface ILoggerFactory
	{
		IInternalLogger LoggerFor(string keyName);
		IInternalLogger LoggerFor(System.Type type);
	}

	[Obsolete("Use NHibernateLogger instead.")]
	public class LoggerProvider
	{
		[Obsolete("Implement INHibernateLoggerFactory and use NHibernateLogger.SetLoggersFactory() instead")]
		public static void SetLoggersFactory(ILoggerFactory loggerFactory)
		{
			var factory = (loggerFactory == null || loggerFactory is NoLoggingLoggerFactory)
				? null
				: (INHibernateLoggerFactory) new LegacyLoggerFactoryAdaptor(loggerFactory);

			NHibernateLogger.SetLoggersFactory(factory);
		}

		[Obsolete("Use NHibernateLogger.For() instead.")]
		public static IInternalLogger LoggerFor(string keyName)
		{
			return NHibernateLogger.LegacyLoggerFactory.LoggerFor(keyName);
		}

		[Obsolete("Use NHibernateLogger.For() instead.")]
		public static IInternalLogger LoggerFor(System.Type type)
		{
			return NHibernateLogger.LegacyLoggerFactory.LoggerFor(type);
		}

		// Since 5.1
		[Obsolete("Used only in Obsolete functions to thunk to INHibernateLoggerFactory")]
		internal class LegacyLoggerFactoryAdaptor : INHibernateLoggerFactory
		{
			internal ILoggerFactory Factory { get; }

			public LegacyLoggerFactoryAdaptor(ILoggerFactory factory)
			{
				Factory = factory;
			}

			public INHibernateLogger LoggerFor(string keyName)
			{
				return new NHibernateLoggerThunk(Factory.LoggerFor(keyName));
			}

			public INHibernateLogger LoggerFor(System.Type type)
			{
				return new NHibernateLoggerThunk(Factory.LoggerFor(type));
			}
		}

		// Since 5.1
		[Obsolete("Used only in Obsolete functions to thunk to INHibernateLoggerFactory")]
		internal class ReverseLegacyLoggerFactoryAdaptor : ILoggerFactory
		{
			private readonly INHibernateLoggerFactory _factory;

			public ReverseLegacyLoggerFactoryAdaptor(INHibernateLoggerFactory factory)
			{
				_factory = factory;
			}

			public IInternalLogger LoggerFor(string keyName)
			{
				return new InternalLoggerThunk(_factory.LoggerFor(keyName));
			}

			public IInternalLogger LoggerFor(System.Type type)
			{
				return new InternalLoggerThunk(_factory.LoggerFor(type));
			}
		}
	}

	// Since 5.1
	[Obsolete("Used only in Obsolete functions to thunk to INHibernateLoggerFactory")]
	internal class NHibernateLoggerThunk : INHibernateLogger
	{
		private readonly IInternalLogger _internalLogger;

		public NHibernateLoggerThunk(IInternalLogger internalLogger)
		{
			_internalLogger = internalLogger ?? throw new ArgumentNullException(nameof(internalLogger));
		}

		public void Log(NHibernateLogLevel logLevel, NHibernateLogValues state, Exception exception)
		{
			if (!IsEnabled(logLevel))
				return;

			switch (logLevel)
			{
				case NHibernateLogLevel.Debug:
				case NHibernateLogLevel.Trace:
					if (exception != null)
						_internalLogger.Debug(state, exception);
					else if (state.Args?.Length > 0)
						_internalLogger.DebugFormat(state.Format, state.Args);
					else
						_internalLogger.Debug(state);
					break;
				case NHibernateLogLevel.Info:
					if (exception != null)
						_internalLogger.Info(state, exception);
					else if (state.Args?.Length > 0)
						_internalLogger.InfoFormat(state.Format, state.Args);
					else
						_internalLogger.Info(state);
					break;
				case NHibernateLogLevel.Warn:
					if (exception != null)
						_internalLogger.Warn(state, exception);
					else if (state.Args?.Length > 0)
						_internalLogger.WarnFormat(state.Format, state.Args);
					else
						_internalLogger.Warn(state);
					break;
				case NHibernateLogLevel.Error:
					if (exception != null)
						_internalLogger.Error(state, exception);
					else if (state.Args?.Length > 0)
						_internalLogger.ErrorFormat(state.Format, state.Args);
					else
						_internalLogger.Error(state);
					break;
				case NHibernateLogLevel.Fatal:
					if (exception != null)
						_internalLogger.Fatal(state, exception);
					else
						_internalLogger.Fatal(state);
					break;
				case NHibernateLogLevel.None:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
			}
		}

		public bool IsEnabled(NHibernateLogLevel logLevel)
		{
			switch (logLevel)
			{
				case NHibernateLogLevel.Trace:
				case NHibernateLogLevel.Debug:
					return _internalLogger.IsDebugEnabled;
				case NHibernateLogLevel.Info:
					return _internalLogger.IsInfoEnabled;
				case NHibernateLogLevel.Warn:
					return _internalLogger.IsWarnEnabled;
				case NHibernateLogLevel.Error:
					return _internalLogger.IsErrorEnabled;
				case NHibernateLogLevel.Fatal:
					return _internalLogger.IsFatalEnabled;
				case NHibernateLogLevel.None:
					return !_internalLogger.IsFatalEnabled;
				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
			}
		}
	}

	// Since 5.1
	[Obsolete("Used only in Obsolete functions to thunk to ILoggerFactory")]
	internal class InternalLoggerThunk : IInternalLogger
	{
		private readonly INHibernateLogger _nhibernateLogger;

		public bool IsErrorEnabled => _nhibernateLogger.IsEnabled(NHibernateLogLevel.Error);
		public bool IsFatalEnabled => _nhibernateLogger.IsEnabled(NHibernateLogLevel.Fatal);
		public bool IsDebugEnabled => _nhibernateLogger.IsEnabled(NHibernateLogLevel.Debug);
		public bool IsInfoEnabled => _nhibernateLogger.IsEnabled(NHibernateLogLevel.Info);
		public bool IsWarnEnabled => _nhibernateLogger.IsEnabled(NHibernateLogLevel.Warn);

		public InternalLoggerThunk(INHibernateLogger nhibernateLogger)
		{
			_nhibernateLogger = nhibernateLogger;
		}

		public void Error(object message)
		{
			_nhibernateLogger.Error(message?.ToString());
		}

		public void Error(object message, Exception exception)
		{
			_nhibernateLogger.Error(exception, message?.ToString());
		}

		public void ErrorFormat(string format, params object[] args)
		{
			_nhibernateLogger.Error(format, args);
		}

		public void Fatal(object message)
		{
			_nhibernateLogger.Fatal(message?.ToString());
		}

		public void Fatal(object message, Exception exception)
		{
			_nhibernateLogger.Fatal(exception, message?.ToString());
		}

		public void Debug(object message)
		{
			_nhibernateLogger.Debug(message?.ToString());
		}

		public void Debug(object message, Exception exception)
		{
			_nhibernateLogger.Debug(exception, message?.ToString());
		}

		public void DebugFormat(string format, params object[] args)
		{
			_nhibernateLogger.Debug(format, args);
		}

		public void Info(object message)
		{
			_nhibernateLogger.Info(message?.ToString());
		}

		public void Info(object message, Exception exception)
		{
			_nhibernateLogger.Info(exception, message?.ToString());
		}

		public void InfoFormat(string format, params object[] args)
		{
			_nhibernateLogger.Info(format, args);
		}

		public void Warn(object message)
		{
			_nhibernateLogger.Warn(message?.ToString());
		}

		public void Warn(object message, Exception exception)
		{
			_nhibernateLogger.Warn(exception, message?.ToString());
		}

		public void WarnFormat(string format, params object[] args)
		{
			_nhibernateLogger.Warn(format, args);
		}
	}

	// Since 5.1
	[Obsolete("To set no-logging, use NHibernateLogger.SetLoggersFactory(default(INHibernateLoggerFactory))")]
	public class NoLoggingLoggerFactory : ILoggerFactory
	{
		private static readonly IInternalLogger Nologging = new NoLoggingInternalLogger();
		public IInternalLogger LoggerFor(string keyName)
		{
			return Nologging;
		}

		public IInternalLogger LoggerFor(System.Type type)
		{
			return Nologging;
		}
	}

	// Since 5.1
	[Obsolete("To set no-logging, use NHibernateLogger.SetLoggersFactory(default(INHibernateLoggerFactory))")]
	public class NoLoggingInternalLogger : IInternalLogger
	{
		public bool IsErrorEnabled
		{
			get { return false; }
		}

		public bool IsFatalEnabled
		{
			get { return false; }
		}

		public bool IsDebugEnabled
		{
			get { return false; }
		}

		public bool IsInfoEnabled
		{
			get { return false; }
		}

		public bool IsWarnEnabled
		{
			get { return false; }
		}

		public void Error(object message)
		{
		}

		public void Error(object message, Exception exception)
		{
		}

		public void ErrorFormat(string format, params object[] args)
		{
		}

		public void Fatal(object message)
		{
		}

		public void Fatal(object message, Exception exception)
		{
		}

		public void Debug(object message)
		{
		}

		public void Debug(object message, Exception exception)
		{
		}

		public void DebugFormat(string format, params object[] args)
		{
		}

		public void Info(object message)
		{
		}

		public void Info(object message, Exception exception)
		{
		}

		public void InfoFormat(string format, params object[] args)
		{
		}

		public void Warn(object message)
		{
		}

		public void Warn(object message, Exception exception)
		{
		}

		public void WarnFormat(string format, params object[] args)
		{
		}
	}
}
