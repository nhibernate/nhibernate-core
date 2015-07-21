using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

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
				throw new ApplicationException("Public constructor was not found for " + loggerFactoryType, ex);
			}
			catch (InvalidCastException ex)
			{
				throw new ApplicationException(loggerFactoryType + "Type does not implement " + typeof (ILoggerFactory), ex);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Unable to instantiate: " + loggerFactoryType, ex);
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
		private static readonly Func<string, object> GetLoggerByNameDelegate;
		private static readonly Func<System.Type, object> GetLoggerByTypeDelegate;
		static Log4NetLoggerFactory()
		{
			GetLoggerByNameDelegate = GetGetLoggerMethodCall<string>();
			GetLoggerByTypeDelegate = GetGetLoggerMethodCall<System.Type>();
		}
		public IInternalLogger LoggerFor(string keyName)
		{
			return new Log4NetLogger(GetLoggerByNameDelegate(keyName));
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
			MethodCallExpression methodCall = Expression.Call(null, method, new Expression[] { resultValue = keyParam });
			return Expression.Lambda<Func<TParameter, object>>(methodCall, new[] { resultValue }).Compile();
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
			IsErrorEnabledDelegate = GetPropertyGetter("IsErrorEnabled");
			IsFatalEnabledDelegate = GetPropertyGetter("IsFatalEnabled");
			IsDebugEnabledDelegate = GetPropertyGetter("IsDebugEnabled");
			IsInfoEnabledDelegate = GetPropertyGetter("IsInfoEnabled");
			IsWarnEnabledDelegate = GetPropertyGetter("IsWarnEnabled");
			ErrorDelegate = GetMethodCallForMessage("Error");
			ErrorExceptionDelegate = GetMethodCallForMessageException("Error");
			ErrorFormatDelegate = GetMethodCallForMessageFormat("ErrorFormat");

			FatalDelegate = GetMethodCallForMessage("Fatal");
			FatalExceptionDelegate = GetMethodCallForMessageException("Fatal");

			DebugDelegate = GetMethodCallForMessage("Debug");
			DebugExceptionDelegate = GetMethodCallForMessageException("Debug");
			DebugFormatDelegate = GetMethodCallForMessageFormat("DebugFormat");

			InfoDelegate = GetMethodCallForMessage("Info");
			InfoExceptionDelegate = GetMethodCallForMessageException("Info");
			InfoFormatDelegate = GetMethodCallForMessageFormat("InfoFormat");

			WarnDelegate = GetMethodCallForMessage("Warn");
			WarnExceptionDelegate = GetMethodCallForMessageException("Warn");
			WarnFormatDelegate = GetMethodCallForMessageFormat("WarnFormat");
		}

		private static Func<object, bool> GetPropertyGetter(string propertyName)
		{
			ParameterExpression funcParam = Expression.Parameter(typeof(object), "l");
			Expression convertedParam = Expression.Convert(funcParam, ILogType);
			Expression property = Expression.Property(convertedParam, propertyName);
			return (Func<object, bool>)Expression.Lambda(property, funcParam).Compile();
		}

		private static Action<object, object> GetMethodCallForMessage(string methodName)
		{
			ParameterExpression loggerParam = Expression.Parameter(typeof(object), "l");
			ParameterExpression messageParam = Expression.Parameter(typeof(object), "o");
			Expression convertedParam = Expression.Convert(loggerParam, ILogType);
			MethodCallExpression methodCall = Expression.Call(convertedParam, ILogType.GetMethod(methodName, new[] { typeof(object) }), messageParam);
			return (Action<object, object>)Expression.Lambda(methodCall, new[] { loggerParam, messageParam }).Compile();
		}

		private static Action<object, object, Exception> GetMethodCallForMessageException(string methodName)
		{
			ParameterExpression loggerParam = Expression.Parameter(typeof(object), "l");
			ParameterExpression messageParam = Expression.Parameter(typeof(object), "o");
			ParameterExpression exceptionParam = Expression.Parameter(typeof(Exception), "e");
			Expression convertedParam = Expression.Convert(loggerParam, ILogType);
			MethodCallExpression methodCall = Expression.Call(convertedParam, ILogType.GetMethod(methodName, new[] { typeof(object), typeof(Exception) }), messageParam, exceptionParam);
			return (Action<object, object, Exception>)Expression.Lambda(methodCall, new[] { loggerParam, messageParam, exceptionParam }).Compile();
		}

		private static Action<object, string, object[]> GetMethodCallForMessageFormat(string methodName)
		{
			ParameterExpression loggerParam = Expression.Parameter(typeof(object), "l");
			ParameterExpression formatParam = Expression.Parameter(typeof(string), "f");
			ParameterExpression parametersParam = Expression.Parameter(typeof(object[]), "p");
			Expression convertedParam = Expression.Convert(loggerParam, ILogType);
			MethodCallExpression methodCall = Expression.Call(convertedParam, ILogType.GetMethod(methodName, new[] { typeof(string), typeof(object[]) }), formatParam, parametersParam);
			return (Action<object, string, object[]>)Expression.Lambda(methodCall, new[] { loggerParam, formatParam, parametersParam }).Compile();
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
