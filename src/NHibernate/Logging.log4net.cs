using System;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Util;

namespace NHibernate
{
#pragma warning disable 618 // ILoggerFactory is obsolete
	/// <summary>
	/// Reflection based log4net logger factory.
	/// </summary>
	public class Log4NetLoggerFactory: ILoggerFactory
#pragma warning restore 618
	{
		private static readonly System.Type LogManagerType;
		private static readonly Func<Assembly, string, object> GetLoggerByNameDelegate;
		private static readonly Func<System.Type, object> GetLoggerByTypeDelegate;

		static Log4NetLoggerFactory()
		{
			LogManagerType = System.Type.GetType("log4net.LogManager, log4net", true);
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
			if (method == null)
				throw new InvalidOperationException($"Unable to find LogManager.GetLogger({typeof(TParameter)})");
			ParameterExpression resultValue;
			ParameterExpression keyParam = Expression.Parameter(typeof(TParameter), "key");
			MethodCallExpression methodCall = Expression.Call(null, method, resultValue = keyParam);
			return Expression.Lambda<Func<TParameter, object>>(methodCall, resultValue).Compile();
		}

		private static Func<Assembly, string, object> GetGetLoggerByNameMethodCall()
		{
			var method = LogManagerType.GetMethod("GetLogger", new[] {typeof(Assembly), typeof(string)});
			if (method == null)
				throw new InvalidOperationException("Unable to find LogManager.GetLogger(Assembly, string)");
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
	public class Log4NetLogger : IInternalLogger
#pragma warning restore 618
	{
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
			var iLogType = System.Type.GetType("log4net.ILog, log4net", true);

			IsErrorEnabledDelegate = DelegateHelper.BuildPropertyGetter<bool>(iLogType, "IsErrorEnabled");
			IsFatalEnabledDelegate = DelegateHelper.BuildPropertyGetter<bool>(iLogType, "IsFatalEnabled");
			IsDebugEnabledDelegate = DelegateHelper.BuildPropertyGetter<bool>(iLogType, "IsDebugEnabled");
			IsInfoEnabledDelegate = DelegateHelper.BuildPropertyGetter<bool>(iLogType, "IsInfoEnabled");
			IsWarnEnabledDelegate = DelegateHelper.BuildPropertyGetter<bool>(iLogType, "IsWarnEnabled");
			ErrorDelegate = DelegateHelper.BuildAction<object>(iLogType, "Error");
			ErrorExceptionDelegate = DelegateHelper.BuildAction<object, Exception>(iLogType, "Error");
			ErrorFormatDelegate = DelegateHelper.BuildAction<string, object[]>(iLogType, "ErrorFormat");

			FatalDelegate = DelegateHelper.BuildAction<object>(iLogType, "Fatal");
			FatalExceptionDelegate = DelegateHelper.BuildAction<object, Exception>(iLogType, "Fatal");

			DebugDelegate = DelegateHelper.BuildAction<object>(iLogType, "Debug");
			DebugExceptionDelegate = DelegateHelper.BuildAction<object, Exception>(iLogType, "Debug");
			DebugFormatDelegate = DelegateHelper.BuildAction<string, object[]>(iLogType, "DebugFormat");

			InfoDelegate = DelegateHelper.BuildAction<object>(iLogType, "Info");
			InfoExceptionDelegate = DelegateHelper.BuildAction<object, Exception>(iLogType, "Info");
			InfoFormatDelegate = DelegateHelper.BuildAction<string, object[]>(iLogType, "InfoFormat");

			WarnDelegate = DelegateHelper.BuildAction<object>(iLogType, "Warn");
			WarnExceptionDelegate = DelegateHelper.BuildAction<object, Exception>(iLogType, "Warn");
			WarnFormatDelegate = DelegateHelper.BuildAction<string, object[]>(iLogType, "WarnFormat");
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
}
