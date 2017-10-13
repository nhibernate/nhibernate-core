using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate
{
	public interface IInternalLogger
	{
		bool IsErrorEnabled { get; }
		bool IsFatalEnabled { get; }
		bool IsDebugEnabled { get; }
		bool IsInfoEnabled { get; }
		bool IsWarnEnabled { get; }

		void Error(object message);

		[Obsolete("Use Error(Exception, string, params object[])")]
		void Error(object message, Exception exception);

		[Obsolete("Use Error(string, params object[])")]
		void ErrorFormat(string format, params object[] args);

		void Fatal(object message);

		[Obsolete("Use Fatal(Exception, string, params object[])")]
		void Fatal(object message, Exception exception);

		void Debug(object message);

		[Obsolete("Use Debug(Exception, string, params object[])")]
		void Debug(object message, Exception exception);

		[Obsolete("Use Debug(string, params object[])")]
		void DebugFormat(string format, params object[] args);

		void Info(object message);

		[Obsolete("Use Info(Exception, string, params object[])")]
		void Info(object message, Exception exception);

		[Obsolete("Use Info(string, params object[])")]
		void InfoFormat(string format, params object[] args);

		void Warn(object message);

		[Obsolete("Use Warn(Exception, string, params object[])")]
		void Warn(object message, Exception exception);

		[Obsolete("Use Warn(string, params object[])")]
		void WarnFormat(string format, params object[] args);
	}

	public interface IInternalLogger2 : IInternalLogger
	{
		void Fatal(Exception exception, string format, params object[] args);
		void Fatal(string format, params object[] args);
		void Error(Exception exception, string format, params object[] args);
		void Error(string format, params object[] args);
		void Warn(Exception exception, string format, params object[] args);
		void Warn(string format, params object[] args);
		void Info(Exception exception, string format, params object[] args);
		void Info(string format, params object[] args);
		void Debug(Exception exception, string format, params object[] args);
		void Debug(string format, params object[] args);
	}

	public interface ILoggerFactory
	{
		IInternalLogger LoggerFor(string keyName);
		IInternalLogger LoggerFor(System.Type type);
	}

	public class LoggerProvider
	{
		private const string NhibernateLoggerConfKey = "nhibernate-logger";
		private readonly ILoggerFactory loggerFactory;
		private static LoggerProvider instance;

		static LoggerProvider()
		{
			string nhibernateLoggerClass = GetNhibernateLoggerClass();
			ILoggerFactory loggerFactory = string.IsNullOrEmpty(nhibernateLoggerClass) ? new NoLoggingLoggerFactory() : GetLoggerFactory(nhibernateLoggerClass);
			SetLoggersFactory(loggerFactory);
		}

		private static ILoggerFactory GetLoggerFactory(string nhibernateLoggerClass)
		{
			ILoggerFactory loggerFactory;
			var loggerFactoryType = System.Type.GetType(nhibernateLoggerClass);
			try
			{
				loggerFactory = (ILoggerFactory) Activator.CreateInstance(loggerFactoryType);
			}
			catch (MissingMethodException ex)
			{
				throw new InstantiationException("Public constructor was not found for " + loggerFactoryType, ex, loggerFactoryType);
			}
			catch (InvalidCastException ex)
			{
				throw new InstantiationException(loggerFactoryType + "Type does not implement " + typeof (ILoggerFactory), ex, loggerFactoryType);
			}
			catch (Exception ex)
			{
				throw new InstantiationException("Unable to instantiate: " + loggerFactoryType, ex, loggerFactoryType);
			}
			return loggerFactory;
		}

		private static string GetNhibernateLoggerClass()
		{
			var nhibernateLogger = ConfigurationManager.AppSettings.Keys.Cast<string>().FirstOrDefault(k => NhibernateLoggerConfKey.Equals(k.ToLowerInvariant()));
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

		public static void SetLoggersFactory(ILoggerFactory loggerFactory)
		{
			instance = new LoggerProvider(loggerFactory);
		}

		private LoggerProvider(ILoggerFactory loggerFactory)
		{
			this.loggerFactory = loggerFactory;
		}

		public static IInternalLogger2 LoggerFor(string keyName)
		{
			var internalLogger = instance.loggerFactory.LoggerFor(keyName);
			if (internalLogger is IInternalLogger2 internalLogger2) return internalLogger2;
			else return new InternalLogger2Thunk(internalLogger);
		}

		public static IInternalLogger2 LoggerFor(System.Type type)
		{
			var internalLogger = instance.loggerFactory.LoggerFor(type);
			if (internalLogger is IInternalLogger2 internalLogger2) return internalLogger2;
			else return new InternalLogger2Thunk(internalLogger);
		}
	}

	public class InternalLogger2Thunk : IInternalLogger2
	{
#pragma warning disable 618
		private readonly IInternalLogger _internalLogger;

		public InternalLogger2Thunk(IInternalLogger internalLogger)
		{
			_internalLogger = internalLogger ?? throw new ArgumentNullException(nameof(internalLogger));
		}

		#region Passthrough to IInternalLogger

		public bool IsErrorEnabled => _internalLogger.IsErrorEnabled;

		public bool IsFatalEnabled => _internalLogger.IsFatalEnabled;

		public bool IsDebugEnabled => _internalLogger.IsDebugEnabled;

		public bool IsInfoEnabled => _internalLogger.IsInfoEnabled;

		public bool IsWarnEnabled => _internalLogger.IsWarnEnabled;

		public void Error(object message)
		{
			_internalLogger.Error(message);
		}

		public void Error(object message, Exception exception)
		{
			_internalLogger.Error(message, exception);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			_internalLogger.ErrorFormat(format, args);
		}

		public void Fatal(object message)
		{
			_internalLogger.Fatal(message);
		}

		public void Fatal(object message, Exception exception)
		{
			_internalLogger.Fatal(message, exception);
		}

		public void Debug(object message)
		{
			_internalLogger.Debug(message);
		}

		public void Debug(object message, Exception exception)
		{
			_internalLogger.Debug(message, exception);
		}

		public void DebugFormat(string format, params object[] args)
		{
			_internalLogger.DebugFormat(format, args);
		}

		public void Info(object message)
		{
			_internalLogger.Info(message);
		}

		public void Info(object message, Exception exception)
		{
			_internalLogger.Info(message, exception);
		}

		public void InfoFormat(string format, params object[] args)
		{
			_internalLogger.InfoFormat(format, args);
		}

		public void Warn(object message)
		{
			_internalLogger.Warn(message);
		}

		public void Warn(object message, Exception exception)
		{
			_internalLogger.Warn(message, exception);
		}

		public void WarnFormat(string format, params object[] args)
		{
			_internalLogger.WarnFormat(format, args);
		}

		#endregion

		public void Fatal(Exception exception, string format, params object[] args)
		{
			if (IsFatalEnabled && args?.Length > 0)
				_internalLogger.Fatal(string.Format(format, args), exception);
			else
				_internalLogger.Fatal(format, exception);
		}

		public void Fatal(string format, params object[] args)
		{
			if (IsFatalEnabled && args?.Length > 0)
				_internalLogger.Fatal(string.Format(format, args));
			else
				_internalLogger.Fatal(format);
		}

		public void Error(Exception exception, string format, params object[] args)
		{
			if (IsErrorEnabled && args?.Length > 0)
				_internalLogger.Error(string.Format(format, args), exception);
			else
				_internalLogger.Error(format);
		}

		public void Error(string format, params object[] args)
		{
			if (args?.Length > 0)
				_internalLogger.ErrorFormat(format, args);
			else
				_internalLogger.Error(format);
		}

		public void Warn(Exception exception, string format, params object[] args)
		{
			if (IsWarnEnabled && args?.Length > 0)
				_internalLogger.Warn(string.Format(format, args), exception);
			else
				_internalLogger.Warn(format);
		}

		public void Warn(string format, params object[] args)
		{
			if (args?.Length > 0)
				_internalLogger.WarnFormat(format, args);
			else
				_internalLogger.Warn(format);
		}

		public void Info(Exception exception, string format, params object[] args)
		{
			if (IsInfoEnabled && args?.Length > 0)
				_internalLogger.Info(string.Format(format, args), exception);
			else
				_internalLogger.Info(format, exception);
		}

		public void Info(string format, params object[] args)
		{
			if (args?.Length > 0)
				_internalLogger.InfoFormat(format, args);
			else
				_internalLogger.Info(format);
		}

		public void Debug(Exception exception, string format, params object[] args)
		{
			if (IsDebugEnabled && args?.Length > 0)
				_internalLogger.Debug(string.Format(format, args), exception);
			else
				_internalLogger.Debug(format, exception);
		}

		public void Debug(string format, params object[] args)
		{
			if (args?.Length > 0)
				_internalLogger.DebugFormat(format, args);
			else
				_internalLogger.Debug(format);
		}
#pragma warning restore 618
	}

	public class NoLoggingLoggerFactory: ILoggerFactory
	{
		private static readonly IInternalLogger2 Nologging = new NoLoggingInternalLogger();
		public IInternalLogger LoggerFor(string keyName)
		{
			return Nologging;
		}

		public IInternalLogger LoggerFor(System.Type type)
		{
			return Nologging;
		}
	}

	public class NoLoggingInternalLogger: IInternalLogger2
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

		public void Fatal(Exception exception, string format, params object[] args)
		{
		}

		public void Fatal(string format, params object[] args)
		{
		}

		public void Error(Exception exception, string format, params object[] args)
		{
		}

		public void Error(string format, params object[] args)
		{
		}

		public void Warn(Exception exception, string format, params object[] args)
		{
		}

		public void Warn(string format, params object[] args)
		{
		}

		public void Info(Exception exception, string format, params object[] args)
		{
		}

		public void Info(string format, params object[] args)
		{
		}

		public void Debug(Exception exception, string format, params object[] args)
		{
		}

		public void Debug(string format, params object[] args)
		{
		}
	}

	public class Log4NetLoggerFactory: ILoggerFactory
	{
		private static readonly System.Type LogManagerType = System.Type.GetType("log4net.LogManager, log4net");
		private static readonly Func<Assembly, string, object> GetLoggerByNameDelegate;
		private static readonly Func<System.Type, object> GetLoggerByTypeDelegate;
		static Log4NetLoggerFactory()
		{
			GetLoggerByNameDelegate = GetGetLoggerByNameMethodCall();
			GetLoggerByTypeDelegate = GetGetLoggerMethodCall<System.Type>();
		}
		public IInternalLogger LoggerFor(string keyName)
		{
			return new Log4NetLogger(GetLoggerByNameDelegate(typeof(Log4NetLoggerFactory).Assembly, keyName));
		}

		public IInternalLogger LoggerFor(System.Type type)
		{
			return new Log4NetLogger(GetLoggerByTypeDelegate(type));
		}

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

	public class Log4NetLogger: IInternalLogger
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
}
