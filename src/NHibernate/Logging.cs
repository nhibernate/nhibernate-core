using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate
{
	[Obsolete("Implement and use NHibernate.IInternalLogger2")]
	public interface IInternalLogger
	{
		bool IsErrorEnabled { get; }
		bool IsFatalEnabled { get; }
		bool IsDebugEnabled { get; }
		bool IsInfoEnabled { get; }
		bool IsWarnEnabled { get; }

		void Error(object message);
		void Error(object message, Exception exception);
		void ErrorFormat(string format, params object[] args);

		void Fatal(object message);
		void Fatal(object message, Exception exception);

		void Debug(object message);
		void Debug(object message, Exception exception);
		void DebugFormat(string format, params object[] args);

		void Info(object message);
		void Info(object message, Exception exception);
		void InfoFormat(string format, params object[] args);

		void Warn(object message);
		void Warn(object message, Exception exception);
		void WarnFormat(string format, params object[] args);
	}

	public interface IInternalLogger2
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

	[Obsolete("Implement INHibernateLoggerFactory instead")]
	public interface ILoggerFactory
	{
		IInternalLogger LoggerFor(string keyName);
		IInternalLogger LoggerFor(System.Type type);
	}

	public interface INHibernateLoggerFactory
	{
		IInternalLogger2 LoggerFor(string keyName);
		IInternalLogger2 LoggerFor(System.Type type);
	}

	public class LoggerProvider
	{
		private const string nhibernateLoggerConfKey = "nhibernate-logger";
		private readonly INHibernateLoggerFactory _loggerFactory;
		private static LoggerProvider _instance;

		static LoggerProvider()
		{
			string nhibernateLoggerClass = GetNhibernateLoggerClass();
			INHibernateLoggerFactory loggerFactory = string.IsNullOrEmpty(nhibernateLoggerClass) ? new NoLoggingLoggerFactory() : GetLoggerFactory(nhibernateLoggerClass);
			SetLoggersFactory(loggerFactory);
		}

		private static INHibernateLoggerFactory GetLoggerFactory(string nhibernateLoggerClass)
		{
			INHibernateLoggerFactory loggerFactory;
			var loggerFactoryType = System.Type.GetType(nhibernateLoggerClass);
			try
			{
				object loadedLoggerFactory = Activator.CreateInstance(loggerFactoryType);
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

		[Obsolete("Implement INHibernateLoggerFactory instead")]
		public static void SetLoggersFactory(ILoggerFactory loggerFactory)
		{
			_instance = new LoggerProvider(new LegacyLoggerFactoryAdaptor(loggerFactory));
		}

		public static void SetLoggersFactory(INHibernateLoggerFactory loggerFactory)
		{
			_instance = new LoggerProvider(loggerFactory);
		}

		private LoggerProvider(INHibernateLoggerFactory loggerFactory)
		{
			this._loggerFactory = loggerFactory;
		}

		public static IInternalLogger2 LoggerFor(string keyName)
		{
			return _instance._loggerFactory.LoggerFor(keyName);
		}

		public static IInternalLogger2 LoggerFor(System.Type type)
		{
			return _instance._loggerFactory.LoggerFor(type);
		}

		[Obsolete("Used only in Obsolete functions to thunk to INHibernateLoggerFactory")]
		private class LegacyLoggerFactoryAdaptor : INHibernateLoggerFactory
		{
			private readonly ILoggerFactory _factory;

			public LegacyLoggerFactoryAdaptor(ILoggerFactory factory)
			{
				_factory = factory;
			}

			public IInternalLogger2 LoggerFor(string keyName)
			{
				return new InternalLogger2Thunk(_factory.LoggerFor(keyName));
			}

			public IInternalLogger2 LoggerFor(System.Type type)
			{
				return new InternalLogger2Thunk(_factory.LoggerFor(type));
			}
		}
	}

	[Obsolete("Used only in Obsolete functions to thunk to INHibernateLoggerFactory")]
	internal class InternalLogger2Thunk : IInternalLogger2
	{
		private readonly IInternalLogger _internalLogger;

		public InternalLogger2Thunk(IInternalLogger internalLogger)
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

	public class NoLoggingLoggerFactory: INHibernateLoggerFactory
	{
		private static readonly IInternalLogger2 Nologging = new NoLoggingInternalLogger();
		public IInternalLogger2 LoggerFor(string keyName)
		{
			return Nologging;
		}

		public IInternalLogger2 LoggerFor(System.Type type)
		{
			return Nologging;
		}
	}

	public class NoLoggingInternalLogger: IInternalLogger2
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

#pragma warning disable 618
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
		public IInternalLogger LoggerFor(string keyName)
		{
			return new Log4NetLogger(GetLoggerByNameDelegate(typeof(Log4NetLoggerFactory).Assembly, keyName));
		}

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

#pragma warning disable 618
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

		private readonly object logger;

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

		public Log4NetLogger(object logger)
		{
			this.logger = logger;
		}

		public bool IsErrorEnabled
		{
			get { return IsErrorEnabledDelegate(logger); }
		}

		public bool IsFatalEnabled
		{
			get { return IsFatalEnabledDelegate(logger); }
		}

		public bool IsDebugEnabled
		{
			get { return IsDebugEnabledDelegate(logger); }
		}

		public bool IsInfoEnabled
		{
			get { return IsInfoEnabledDelegate(logger); }
		}

		public bool IsWarnEnabled
		{
			get { return IsWarnEnabledDelegate(logger); }
		}

		public void Error(object message)
		{
			if (IsErrorEnabled)
				ErrorDelegate(logger, message);
		}

		public void Error(object message, Exception exception)
		{
			if (IsErrorEnabled)
				ErrorExceptionDelegate(logger, message, exception);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			if (IsErrorEnabled)
				ErrorFormatDelegate(logger, format, args);
		}

		public void Fatal(object message)
		{
			if (IsFatalEnabled)
				FatalDelegate(logger, message);
		}

		public void Fatal(object message, Exception exception)
		{
			if (IsFatalEnabled)
				FatalExceptionDelegate(logger, message, exception);
		}

		public void Debug(object message)
		{
			if (IsDebugEnabled)
				DebugDelegate(logger, message);
		}

		public void Debug(object message, Exception exception)
		{
			if (IsDebugEnabled)
				DebugExceptionDelegate(logger, message, exception);
		}

		public void DebugFormat(string format, params object[] args)
		{
			if (IsDebugEnabled)
				DebugFormatDelegate(logger, format, args);
		}

		public void Info(object message)
		{
			if (IsInfoEnabled)
				InfoDelegate(logger, message);
		}

		public void Info(object message, Exception exception)
		{
			if (IsInfoEnabled)
				InfoExceptionDelegate(logger, message, exception);
		}

		public void InfoFormat(string format, params object[] args)
		{
			if (IsInfoEnabled)
				InfoFormatDelegate(logger, format, args);
		}

		public void Warn(object message)
		{
			if (IsWarnEnabled)
				WarnDelegate(logger, message);
		}

		public void Warn(object message, Exception exception)
		{
			if (IsWarnEnabled)
				WarnExceptionDelegate(logger, message, exception);
		}

		public void WarnFormat(string format, params object[] args)
		{
			if (IsWarnEnabled)
				WarnFormatDelegate(logger, format, args);
		}
	}

	public static class InternalLogger2Extensions
	{
		public static bool IsDebugEnabled(this IInternalLogger2 logger) => logger.IsEnabled(InternalLogLevel.Debug);
		public static bool IsInfoEnabled(this IInternalLogger2 logger) => logger.IsEnabled(InternalLogLevel.Info);
		public static bool IsWarnEnabled(this IInternalLogger2 logger) => logger.IsEnabled(InternalLogLevel.Warn);
		public static bool IsErrorEnabled(this IInternalLogger2 logger) => logger.IsEnabled(InternalLogLevel.Error);
		public static bool IsFatalEnabled(this IInternalLogger2 logger) => logger.IsEnabled(InternalLogLevel.Fatal);

		public static void Fatal(this IInternalLogger2 logger, Exception exception, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Fatal, new InternalLogValues(format, args), exception);
		}

		public static void Fatal(this IInternalLogger2 logger, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Fatal, new InternalLogValues(format, args), null);
		}

		public static void Error(this IInternalLogger2 logger, Exception exception, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Error, new InternalLogValues(format, args), exception);
		}

		public static void Error(this IInternalLogger2 logger, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Error, new InternalLogValues(format, args), null);
		}

		public static void Warn(this IInternalLogger2 logger, Exception exception, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Warn, new InternalLogValues(format, args), exception);
		}

		public static void Warn(this IInternalLogger2 logger, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Warn, new InternalLogValues(format, args), null);
		}

		public static void Info(this IInternalLogger2 logger, Exception exception, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Info, new InternalLogValues(format, args), exception);
		}

		public static void Info(this IInternalLogger2 logger, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Info, new InternalLogValues(format, args), null);
		}

		public static void Debug(this IInternalLogger2 logger, Exception exception, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Debug, new InternalLogValues(format, args), exception);
		}

		public static void Debug(this IInternalLogger2 logger, string format, params object[] args)
		{
			logger.Log(InternalLogLevel.Debug, new InternalLogValues(format, args), null);
		}


		// catch any method calls with an Exception argument second as they would otherwise silently be consumed by `parms object[] args`.

		/// <summary>
		/// Don't use or implement, simply throw new NotImplementedException();
		/// </summary>
		[Obsolete("Use Fatal(Exception, string, params object[])", true)]
		public static void Fatal(this IInternalLogger2 logger, string message, Exception ex) => ThrowNotImplemented();

		/// <summary>
		/// Don't use or implement, simply throw new NotImplementedException();
		/// </summary>
		[Obsolete("Use Error(Exception, string, params object[])", true)]
		public static void Error(this IInternalLogger2 logger, string message, Exception ex) => ThrowNotImplemented();

		/// <summary>
		/// Don't use or implement, simply throw new NotImplementedException();
		/// </summary>
		[Obsolete("Use Warn(Exception, string, params object[])", true)]
		public static void Warn(this IInternalLogger2 logger, string message, Exception ex) => ThrowNotImplemented();

		/// <summary>
		/// Don't use or implement, simply throw new NotImplementedException();
		/// </summary>
		[Obsolete("Use Info(Exception, string, params object[])", true)]
		public static void Info(this IInternalLogger2 logger, string message, Exception ex) => ThrowNotImplemented();

		/// <summary>
		/// Don't use or implement, simply throw new NotImplementedException();
		/// </summary>
		[Obsolete("Use Debug(Exception, string, params object[])", true)]
		public static void Debug(this IInternalLogger2 logger, string message, Exception ex) => ThrowNotImplemented();

		private static void ThrowNotImplemented()
		{
			throw new NotImplementedException("Should not have compiled with call to this method");
		}
	}

	public class InternalLogValues
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
