using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Util;

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

	public interface INHibernateLogger
	{
		/// <summary>Writes a log entry.</summary>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="state">The entry to be written.</param>
		/// <param name="exception">The exception related to this entry.</param>
		void Log(InternalLogLevel logLevel, InternalLogValues state, Exception exception);

		/// <summary>
		/// Checks if the given <paramref name="logLevel" /> is enabled.
		/// </summary>
		/// <param name="logLevel">level to be checked.</param>
		/// <returns><c>true</c> if enabled.</returns>
		bool IsEnabled(InternalLogLevel logLevel);
	}

	// Since 5.1
	[Obsolete("Implement INHibernateLoggerFactory instead")]
	public interface ILoggerFactory
	{
		IInternalLogger LoggerFor(string keyName);
		IInternalLogger LoggerFor(System.Type type);
	}

	/// <summary>
	/// Factory interface for providing a <see cref="INHibernateLogger"/>.
	/// </summary>
	public interface INHibernateLoggerFactory
	{
		/// <summary>
		/// Get a logger for the given log key.
		/// </summary>
		/// <param name="keyName">The log key.</param>
		/// <returns>A NHibernate logger.</returns>
		INHibernateLogger LoggerFor(string keyName);
		/// <summary>
		/// Get a logger using the given type as log key.
		/// </summary>
		/// <param name="type">The type to use as log key.</param>
		/// <returns>A NHibernate logger.</returns>
		INHibernateLogger LoggerFor(System.Type type);
	}

	/// <summary>
	/// Provide methods for getting NHibernate loggers according to supplied <see cref="INHibernateLoggerFactory"/>.
	/// </summary>
	/// <remarks>
	/// By default, it will use a <see cref="Log4NetLoggerFactory"/> if log4net is available, otherwise it will
	/// use a <see cref="NoLoggingNHibernateLoggerFactory"/>.
	/// </remarks>
	public class LoggerProvider
	{
		private const string nhibernateLoggerConfKey = "nhibernate-logger";
		private readonly INHibernateLoggerFactory _loggerFactory;
		private static LoggerProvider _instance;

#pragma warning disable 618
		private readonly ILoggerFactory _legacyLoggerFactory;
#pragma warning restore 618

		static LoggerProvider()
		{
			var nhibernateLoggerClass = GetNhibernateLoggerClass();
			var loggerFactory = string.IsNullOrEmpty(nhibernateLoggerClass) ? null : GetLoggerFactory(nhibernateLoggerClass);
			SetLoggersFactory(loggerFactory);
		}

		private static INHibernateLoggerFactory GetLoggerFactory(string nhibernateLoggerClass)
		{
			INHibernateLoggerFactory loggerFactory;
			var loggerFactoryType = System.Type.GetType(nhibernateLoggerClass);
			try
			{
				var loadedLoggerFactory = Activator.CreateInstance(loggerFactoryType);
#pragma warning disable 618
				if (loadedLoggerFactory is ILoggerFactory oldStyleFactory)
				{
					loggerFactory = new LegacyLoggerFactoryAdaptor(oldStyleFactory);
				}
#pragma warning restore 618
				else
				{
					loggerFactory = (INHibernateLoggerFactory) loadedLoggerFactory;
				}
			}
			catch (MissingMethodException ex)
			{
				throw new InstantiationException("Public constructor was not found for " + loggerFactoryType, ex, loggerFactoryType);
			}
			catch (InvalidCastException ex)
			{
#pragma warning disable 618
				throw new InstantiationException(loggerFactoryType + "Type does not implement " + typeof(INHibernateLoggerFactory) + " or " + typeof (ILoggerFactory), ex, loggerFactoryType);
#pragma warning restore 618
			}
			catch (Exception ex)
			{
				throw new InstantiationException("Unable to instantiate: " + loggerFactoryType, ex, loggerFactoryType);
			}
			return loggerFactory;
		}

		private static string GetNhibernateLoggerClass()
		{
			var nhibernateLogger = ConfigurationManager.AppSettings.Keys.Cast<string>().FirstOrDefault(k => nhibernateLoggerConfKey.Equals(k.ToLowerInvariant()));
			string nhibernateLoggerClass = null;
			if (string.IsNullOrEmpty(nhibernateLogger))
			{
				// look for log4net.dll
				string baseDir = AppDomain.CurrentDomain.BaseDirectory;
				string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
				string binPath = relativeSearchPath == null ? baseDir : Path.Combine(baseDir, relativeSearchPath);
				string log4NetDllPath = binPath == null ? "log4net.dll" : Path.Combine(binPath, "log4net.dll");

				if (File.Exists(log4NetDllPath) || AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == "log4net"))
				{
					nhibernateLoggerClass = typeof (Log4NetLoggerFactory).AssemblyQualifiedName;
				}
			}
			else
			{
				nhibernateLoggerClass = ConfigurationManager.AppSettings[nhibernateLogger];
			}
			return nhibernateLoggerClass;
		}

		// Since 5.1
		[Obsolete("Implement INHibernateLoggerFactory instead")]
		public static void SetLoggersFactory(ILoggerFactory loggerFactory)
		{
			var factory = loggerFactory != null
				? (INHibernateLoggerFactory) new LegacyLoggerFactoryAdaptor(loggerFactory)
				: new NoLoggingNHibernateLoggerFactory();
			SetLoggersFactory(factory);
		}

		/// <summary>
		/// Specify the logger factory to use for building loggers.
		/// </summary>
		/// <param name="loggerFactory">A logger factory.</param>
		public static void SetLoggersFactory(INHibernateLoggerFactory loggerFactory)
		{
			_instance = new LoggerProvider(loggerFactory ?? new NoLoggingNHibernateLoggerFactory());
		}

		private LoggerProvider(INHibernateLoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;

#pragma warning disable 618
			_legacyLoggerFactory = loggerFactory is LegacyLoggerFactoryAdaptor legacy
				? legacy.Factory
				: new ReverseLegacyLoggerFactoryAdaptor(loggerFactory);
#pragma warning restore 618
		}

		/// <summary>
		/// Get a logger for the given log key.
		/// </summary>
		/// <param name="keyName">The log key.</param>
		/// <returns>A NHibernate logger.</returns>
		// Since 5.1
		[Obsolete("Use LoggerProvider.For() instead.")]
		public static IInternalLogger LoggerFor(string keyName)
		{
			return _instance._legacyLoggerFactory.LoggerFor(keyName);
		}

		/// <summary>
		/// Get a logger using the given type as log key.
		/// </summary>
		/// <param name="type">The type to use as log key.</param>
		/// <returns>A NHibernate logger.</returns>
		// Since 5.1
		[Obsolete("Use LoggerProvider.For() instead.")]
		public static IInternalLogger LoggerFor(System.Type type)
		{
			return _instance._legacyLoggerFactory.LoggerFor(type);
		}

		/// <summary>
		/// Get a logger for the given log key.
		/// </summary>
		/// <param name="keyName">The log key.</param>
		/// <returns>A NHibernate logger.</returns>
		public static INHibernateLogger For(string keyName)
		{
			return _instance._loggerFactory.LoggerFor(keyName);
		}

		/// <summary>
		/// Get a logger using the given type as log key.
		/// </summary>
		/// <param name="type">The type to use as log key.</param>
		/// <returns>A NHibernate logger.</returns>
		public static INHibernateLogger For(System.Type type)
		{
			return _instance._loggerFactory.LoggerFor(type);
		}

		// Since 5.1
		[Obsolete("Used only in Obsolete functions to thunk to INHibernateLoggerFactory")]
		private class LegacyLoggerFactoryAdaptor : INHibernateLoggerFactory
		{
			public ILoggerFactory Factory { get; }

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
		private class ReverseLegacyLoggerFactoryAdaptor : ILoggerFactory
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

		public void Log(InternalLogLevel logLevel, InternalLogValues state, Exception exception)
		{
			if (!IsEnabled(logLevel))
				return;

			switch (logLevel)
			{
				case InternalLogLevel.Debug:
				case InternalLogLevel.Trace:
					if (exception != null)
						_internalLogger.Debug(state, exception);
					else if (state.Args?.Length > 0)
						_internalLogger.DebugFormat(state.Format, state.Args);
					else
						_internalLogger.Debug(state);
					break;
				case InternalLogLevel.Info:
					if (exception != null)
						_internalLogger.Info(state, exception);
					else if (state.Args?.Length > 0)
						_internalLogger.InfoFormat(state.Format, state.Args);
					else
						_internalLogger.Info(state);
					break;
				case InternalLogLevel.Warn:
					if (exception != null)
						_internalLogger.Warn(state, exception);
					else if (state.Args?.Length > 0)
						_internalLogger.WarnFormat(state.Format, state.Args);
					else
						_internalLogger.Warn(state);
					break;
				case InternalLogLevel.Error:
					if (exception != null)
						_internalLogger.Error(state, exception);
					else if (state.Args?.Length > 0)
						_internalLogger.ErrorFormat(state.Format, state.Args);
					else
						_internalLogger.Error(state);
					break;
				case InternalLogLevel.Fatal:
					if (exception != null)
						_internalLogger.Fatal(state, exception);
					else
						_internalLogger.Fatal(state);
					break;
				case InternalLogLevel.None:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
			}
		}

		public bool IsEnabled(InternalLogLevel logLevel)
		{
			switch (logLevel)
			{
				case InternalLogLevel.Trace:
				case InternalLogLevel.Debug:
					return _internalLogger.IsDebugEnabled;
				case InternalLogLevel.Info:
					return _internalLogger.IsInfoEnabled;
				case InternalLogLevel.Warn:
					return _internalLogger.IsWarnEnabled;
				case InternalLogLevel.Error:
					return _internalLogger.IsErrorEnabled;
				case InternalLogLevel.Fatal:
					return _internalLogger.IsFatalEnabled;
				case InternalLogLevel.None:
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

		public bool IsErrorEnabled => _nhibernateLogger.IsEnabled(InternalLogLevel.Error);
		public bool IsFatalEnabled => _nhibernateLogger.IsEnabled(InternalLogLevel.Fatal);
		public bool IsDebugEnabled => _nhibernateLogger.IsEnabled(InternalLogLevel.Debug);
		public bool IsInfoEnabled => _nhibernateLogger.IsEnabled(InternalLogLevel.Info);
		public bool IsWarnEnabled => _nhibernateLogger.IsEnabled(InternalLogLevel.Warn);

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

	internal class NoLoggingNHibernateLoggerFactory: INHibernateLoggerFactory
	{
		private static readonly INHibernateLogger Nologging = new NoLoggingNHibernateLogger();
		public INHibernateLogger LoggerFor(string keyName)
		{
			return Nologging;
		}

		public INHibernateLogger LoggerFor(System.Type type)
		{
			return Nologging;
		}
	}

	internal class NoLoggingNHibernateLogger: INHibernateLogger
	{
		public void Log(InternalLogLevel logLevel, InternalLogValues state, Exception exception)
		{
		}

		public bool IsEnabled(InternalLogLevel logLevel)
		{
			if (logLevel == InternalLogLevel.None) return true;

			return false;
		}
	}

	// Since 5.1
	[Obsolete("To set no-logging, use LoggerProvider.SetLoggersFactory(default(INHibernateLoggerFactory))")]
	public class NoLoggingLoggerFactory: ILoggerFactory
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
	[Obsolete("To set no-logging, use LoggerProvider.SetLoggersFactory(default(INHibernateLoggerFactory))")]
	public class NoLoggingInternalLogger: IInternalLogger
	{
		public bool IsErrorEnabled
		{
			get { return false;}
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

#pragma warning disable 618 // ILoggerFactory is obsolete
	/// <summary>
	/// Reflection based log4net logger factory.
	/// </summary>
	public class Log4NetLoggerFactory: ILoggerFactory
#pragma warning restore 618
	{
		private static readonly System.Type LogManagerType = System.Type.GetType("log4net.LogManager, log4net");
		private static readonly Func<Assembly, string, object> GetLoggerByNameDelegate;
		private static readonly Func<System.Type, object> GetLoggerByTypeDelegate;

		static Log4NetLoggerFactory()
		{
			GetLoggerByNameDelegate = GetGetLoggerByNameMethodCall();
			GetLoggerByTypeDelegate = GetGetLoggerMethodCall<System.Type>();
		}

#pragma warning disable 618
		[Obsolete("Use this as an INHibernateLoggerFactory instead.")]
		public IInternalLogger LoggerFor(string keyName)
		{
			return new Log4NetLogger(GetLoggerByNameDelegate(typeof(Log4NetLoggerFactory).Assembly, keyName));
		}

		[Obsolete("Use this as an INHibernateLoggerFactory instead.")]
		public IInternalLogger LoggerFor(System.Type type)
		{
			return new Log4NetLogger(GetLoggerByTypeDelegate(type));
		}
#pragma warning restore 618

		private static Func<TParameter, object> GetGetLoggerMethodCall<TParameter>()
		{
			var method = LogManagerType.GetMethod("GetLogger", new[] { typeof(TParameter) });
			ParameterExpression resultValue;
			ParameterExpression keyParam = Expression.Parameter(typeof(TParameter), "key");
			MethodCallExpression methodCall = Expression.Call(null, method, resultValue = keyParam);
			return Expression.Lambda<Func<TParameter, object>>(methodCall, resultValue).Compile();
		}

		private static Func<Assembly, string, object> GetGetLoggerByNameMethodCall()
		{
			var method = LogManagerType.GetMethod("GetLogger", new[] {typeof(Assembly), typeof(string)});
			ParameterExpression nameParam = Expression.Parameter(typeof(string), "name");
			ParameterExpression repositoryAssemblyParam = Expression.Parameter(typeof(Assembly), "repositoryAssembly");
			MethodCallExpression methodCall = Expression.Call(null, method, repositoryAssemblyParam, nameParam);
			return Expression.Lambda<Func<Assembly, string, object>>(methodCall, repositoryAssemblyParam, nameParam).Compile();
		}
	}

#pragma warning disable 618 // IInternalLogger is obsolete, to be removed in a upcoming major version
	/// <summary>
	/// Reflection based log4net logger.
	/// </summary>
	public class Log4NetLogger: IInternalLogger
#pragma warning restore 618
	{
		private static readonly System.Type ILogType = System.Type.GetType("log4net.ILog, log4net");
		private static readonly Func<object, bool> IsErrorEnabledDelegate;
		private static readonly Func<object, bool> IsFatalEnabledDelegate;
		private static readonly Func<object, bool> IsDebugEnabledDelegate;
		private static readonly Func<object, bool> IsInfoEnabledDelegate;
		private static readonly Func<object, bool> IsWarnEnabledDelegate;

		private static readonly Action<object, object> ErrorDelegate;
		private static readonly Action<object, object, Exception> ErrorExceptionDelegate;
		private static readonly Action<object, string, object[]> ErrorFormatDelegate;

		private static readonly Action<object, object> FatalDelegate;
		private static readonly Action<object, object, Exception> FatalExceptionDelegate;

		private static readonly Action<object, object> DebugDelegate;
		private static readonly Action<object, object, Exception> DebugExceptionDelegate;
		private static readonly Action<object, string, object[]> DebugFormatDelegate;

		private static readonly Action<object, object> InfoDelegate;
		private static readonly Action<object, object, Exception> InfoExceptionDelegate;
		private static readonly Action<object, string, object[]> InfoFormatDelegate;

		private static readonly Action<object, object> WarnDelegate;
		private static readonly Action<object, object, Exception> WarnExceptionDelegate;
		private static readonly Action<object, string, object[]> WarnFormatDelegate;

		private readonly object _logger;

		static Log4NetLogger()
		{
			IsErrorEnabledDelegate = DelegateHelper.BuildPropertyGetter<bool>(ILogType, "IsErrorEnabled");
			IsFatalEnabledDelegate = DelegateHelper.BuildPropertyGetter<bool>(ILogType, "IsFatalEnabled");
			IsDebugEnabledDelegate = DelegateHelper.BuildPropertyGetter<bool>(ILogType, "IsDebugEnabled");
			IsInfoEnabledDelegate = DelegateHelper.BuildPropertyGetter<bool>(ILogType, "IsInfoEnabled");
			IsWarnEnabledDelegate = DelegateHelper.BuildPropertyGetter<bool>(ILogType, "IsWarnEnabled");
			ErrorDelegate = DelegateHelper.BuildAction<object>(ILogType, "Error");
			ErrorExceptionDelegate = DelegateHelper.BuildAction<object, Exception>(ILogType, "Error");
			ErrorFormatDelegate = DelegateHelper.BuildAction<string, object[]>(ILogType, "ErrorFormat");

			FatalDelegate = DelegateHelper.BuildAction<object>(ILogType, "Fatal");
			FatalExceptionDelegate = DelegateHelper.BuildAction<object, Exception>(ILogType, "Fatal");

			DebugDelegate = DelegateHelper.BuildAction<object>(ILogType, "Debug");
			DebugExceptionDelegate = DelegateHelper.BuildAction<object, Exception>(ILogType, "Debug");
			DebugFormatDelegate = DelegateHelper.BuildAction<string, object[]>(ILogType, "DebugFormat");

			InfoDelegate = DelegateHelper.BuildAction<object>(ILogType, "Info");
			InfoExceptionDelegate = DelegateHelper.BuildAction<object, Exception>(ILogType, "Info");
			InfoFormatDelegate = DelegateHelper.BuildAction<string, object[]>(ILogType, "InfoFormat");

			WarnDelegate = DelegateHelper.BuildAction<object>(ILogType, "Warn");
			WarnExceptionDelegate = DelegateHelper.BuildAction<object, Exception>(ILogType, "Warn");
			WarnFormatDelegate = DelegateHelper.BuildAction<string, object[]>(ILogType, "WarnFormat");
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="logger">The <c>log4net.ILog</c> logger to use for logging.</param>
		public Log4NetLogger(object logger)
		{
			_logger = logger;
		}

		public bool IsErrorEnabled
		{
			get { return IsErrorEnabledDelegate(_logger); }
		}

		public bool IsFatalEnabled
		{
			get { return IsFatalEnabledDelegate(_logger); }
		}

		public bool IsDebugEnabled
		{
			get { return IsDebugEnabledDelegate(_logger); }
		}

		public bool IsInfoEnabled
		{
			get { return IsInfoEnabledDelegate(_logger); }
		}

		public bool IsWarnEnabled
		{
			get { return IsWarnEnabledDelegate(_logger); }
		}

		public void Error(object message)
		{
			if (IsErrorEnabled)
				ErrorDelegate(_logger, message);
		}

		public void Error(object message, Exception exception)
		{
			if (IsErrorEnabled)
				ErrorExceptionDelegate(_logger, message, exception);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			if (IsErrorEnabled)
				ErrorFormatDelegate(_logger, format, args);
		}

		public void Fatal(object message)
		{
			if (IsFatalEnabled)
				FatalDelegate(_logger, message);
		}

		public void Fatal(object message, Exception exception)
		{
			if (IsFatalEnabled)
				FatalExceptionDelegate(_logger, message, exception);
		}

		public void Debug(object message)
		{
			if (IsDebugEnabled)
				DebugDelegate(_logger, message);
		}

		public void Debug(object message, Exception exception)
		{
			if (IsDebugEnabled)
				DebugExceptionDelegate(_logger, message, exception);
		}

		public void DebugFormat(string format, params object[] args)
		{
			if (IsDebugEnabled)
				DebugFormatDelegate(_logger, format, args);
		}

		public void Info(object message)
		{
			if (IsInfoEnabled)
				InfoDelegate(_logger, message);
		}

		public void Info(object message, Exception exception)
		{
			if (IsInfoEnabled)
				InfoExceptionDelegate(_logger, message, exception);
		}

		public void InfoFormat(string format, params object[] args)
		{
			if (IsInfoEnabled)
				InfoFormatDelegate(_logger, format, args);
		}

		public void Warn(object message)
		{
			if (IsWarnEnabled)
				WarnDelegate(_logger, message);
		}

		public void Warn(object message, Exception exception)
		{
			if (IsWarnEnabled)
				WarnExceptionDelegate(_logger, message, exception);
		}

		public void WarnFormat(string format, params object[] args)
		{
			if (IsWarnEnabled)
				WarnFormatDelegate(_logger, format, args);
		}
	}

	/// <summary>
	/// Extensions method for logging.
	/// </summary>
	public static class NHibernateLoggerExtensions
	{
		public static bool IsDebugEnabled(this INHibernateLogger logger) => logger.IsEnabled(InternalLogLevel.Debug);
		public static bool IsInfoEnabled(this INHibernateLogger logger) => logger.IsEnabled(InternalLogLevel.Info);
		public static bool IsWarnEnabled(this INHibernateLogger logger) => logger.IsEnabled(InternalLogLevel.Warn);
		public static bool IsErrorEnabled(this INHibernateLogger logger) => logger.IsEnabled(InternalLogLevel.Error);
		public static bool IsFatalEnabled(this INHibernateLogger logger) => logger.IsEnabled(InternalLogLevel.Fatal);

		public static void Fatal(this INHibernateLogger logger, Exception exception, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Fatal, new InternalLogValues(format, args), exception);
		}

		public static void Fatal(this INHibernateLogger logger, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Fatal, new InternalLogValues(format, args), null);
		}

		public static void Error(this INHibernateLogger logger, Exception exception, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Error, new InternalLogValues(format, args), exception);
		}

		public static void Error(this INHibernateLogger logger, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Error, new InternalLogValues(format, args), null);
		}

		public static void Warn(this INHibernateLogger logger, Exception exception, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Warn, new InternalLogValues(format, args), exception);
		}

		public static void Warn(this INHibernateLogger logger, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Warn, new InternalLogValues(format, args), null);
		}

		public static void Info(this INHibernateLogger logger, Exception exception, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Info, new InternalLogValues(format, args), exception);
		}

		public static void Info(this INHibernateLogger logger, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Info, new InternalLogValues(format, args), null);
		}

		public static void Debug(this INHibernateLogger logger, Exception exception, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Debug, new InternalLogValues(format, args), exception);
		}

		public static void Debug(this INHibernateLogger logger, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Debug, new InternalLogValues(format, args), null);
		}


		// catch any method calls with an Exception argument second as they would otherwise silently be consumed by `params object[] args`.

		/// <summary>
		/// Throws NotImplementedException. Calling this method is an error. Please use methods taking the exception as first argument instead.
		/// </summary>
		[Obsolete("Use Fatal(Exception, string, params object[])", true)]
		public static void Fatal(this INHibernateLogger logger, string message, Exception ex) => ThrowNotImplemented();

		/// <summary>
		/// Throws NotImplementedException. Calling this method is an error. Please use methods taking the exception as first argument instead.
		/// </summary>
		[Obsolete("Use Error(Exception, string, params object[])", true)]
		public static void Error(this INHibernateLogger logger, string message, Exception ex) => ThrowNotImplemented();

		/// <summary>
		/// Throws NotImplementedException. Calling this method is an error. Please use methods taking the exception as first argument instead.
		/// </summary>
		[Obsolete("Use Warn(Exception, string, params object[])", true)]
		public static void Warn(this INHibernateLogger logger, string message, Exception ex) => ThrowNotImplemented();

		/// <summary>
		/// Throws NotImplementedException. Calling this method is an error. Please use methods taking the exception as first argument instead.
		/// </summary>
		[Obsolete("Use Info(Exception, string, params object[])", true)]
		public static void Info(this INHibernateLogger logger, string message, Exception ex) => ThrowNotImplemented();

		/// <summary>
		/// Throws NotImplementedException. Calling this method is an error. Please use methods taking the exception as first argument instead.
		/// </summary>
		[Obsolete("Use Debug(Exception, string, params object[])", true)]
		public static void Debug(this INHibernateLogger logger, string message, Exception ex) => ThrowNotImplemented();

		private static void ThrowNotImplemented()
		{
			throw new NotImplementedException("Should not have compiled with call to this method");
		}
	}

	public struct InternalLogValues
	{
		private readonly string _format;
		private readonly object[] _args;

		public InternalLogValues(string format, object[] args)
		{
			_format = format ?? "[Null]";
			_args = args;
		}

		public string Format => _format;
		public object[] Args => _args;

		public override string ToString()
		{
			return _args?.Length > 0 ? string.Format(_format, _args) : Format;
		}
	}

	/// <summary>Defines logging severity levels.</summary>
	public enum InternalLogLevel
	{
		Trace,
		Debug,
		Info,
		Warn,
		Error,
		Fatal,
		None,
	}
}
