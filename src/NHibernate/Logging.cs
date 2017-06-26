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

		public static IInternalLogger LoggerFor(string keyName)
		{
			return instance.loggerFactory.LoggerFor(keyName);
		}

		public static IInternalLogger LoggerFor(System.Type type)
		{
			return instance.loggerFactory.LoggerFor(type);
		}
	}

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
