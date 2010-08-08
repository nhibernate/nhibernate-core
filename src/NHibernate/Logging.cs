using System;
using log4net;

namespace NHibernate
{
	public interface ILogger
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

	public class LogggerProvider
	{
		private readonly Func<string, ILogger> loggerByKeyGetter;
		private readonly Func<System.Type, ILogger> loggerByTypeGetter;
		private static LogggerProvider instance;
		static LogggerProvider()
		{
			SetLoggersFactoryDelegates(key => new Log4NetLogger(LogManager.GetLogger(key)), type => new Log4NetLogger(LogManager.GetLogger(type)));
		}

		public static void SetLoggersFactoryDelegates(Func<string, ILogger> loggerByKeyGetter, Func<System.Type, ILogger> loggerByTypeGetter)
		{
			instance = new LogggerProvider(loggerByKeyGetter, loggerByTypeGetter);
		}

		private LogggerProvider(Func<string, ILogger> loggerByKeyGetter, Func<System.Type, ILogger> loggerByTypeGetter)
		{
			this.loggerByKeyGetter = loggerByKeyGetter;
			this.loggerByTypeGetter = loggerByTypeGetter;
		}

		public static ILogger LoggerFor(string keyName)
		{
			return instance.loggerByKeyGetter(keyName);
		}

		public static ILogger LoggerFor(System.Type type)
		{
			return instance.loggerByTypeGetter(type);
		}
	}

	public class Log4NetLogger: ILogger
	{
		private readonly ILog logger;
		private readonly Func<ILog, bool> isErrorEnabledDelegate = l => l.IsErrorEnabled;
		private readonly Func<ILog, bool> isFatalEnabledDelegate = l => l.IsFatalEnabled;
		private readonly Func<ILog, bool> isDebugEnabledDelegate = l => l.IsDebugEnabled;
		private readonly Func<ILog, bool> isInfoEnabledDelegate = l => l.IsInfoEnabled;
		private readonly Func<ILog, bool> isWarnEnabledDelegate = l => l.IsWarnEnabled;

		private readonly Action<ILog, object> errorDelegate= (l,o) => l.Error(o);
		private readonly Action<ILog, object,Exception> errorExceptionDelegate=(l,o,e)=> l.Error(o,e);
		private readonly Action<ILog, string , object[]> errorFormatDelegate= (l,f,p)=>l.ErrorFormat(f,p);

		private readonly Action<ILog, object> fatalDelegate= (l,o) => l.Fatal(o);
		private readonly Action<ILog, object,Exception> fatalExceptionDelegate=(l,o,e)=> l.Fatal(o,e);

		private readonly Action<ILog, object> debugDelegate= (l,o) => l.Debug(o);
		private readonly Action<ILog, object, Exception> debugExceptionDelegate = (l, o, e) => l.Debug(o, e);
		private readonly Action<ILog, string, object[]> debugFormatDelegate = (l, f, p) => l.DebugFormat(f, p);

		private readonly Action<ILog, object> infoDelegate= (l,o) => l.Info(o);
		private readonly Action<ILog, object, Exception> infoExceptionDelegate = (l, o, e) => l.Info(o, e);
		private readonly Action<ILog, string, object[]> infoFormatDelegate = (l, f, p) => l.InfoFormat(f, p);

		private readonly Action<ILog, object> warnDelegate= (l,o) => l.Warn(o);
		private readonly Action<ILog, object, Exception> warnExceptionDelegate = (l, o, e) => l.Warn(o, e);
		private readonly Action<ILog, string, object[]> warnFormatDelegate = (l, f, p) => l.WarnFormat(f, p);

		public Log4NetLogger(ILog logger)
		{
			this.logger = logger;
		}

		public bool IsErrorEnabled
		{
			get { return isErrorEnabledDelegate(logger); }
		}

		public bool IsFatalEnabled
		{
			get { return isFatalEnabledDelegate(logger); }
		}

		public bool IsDebugEnabled
		{
			get { return isDebugEnabledDelegate(logger); }
		}

		public bool IsInfoEnabled
		{
			get { return isInfoEnabledDelegate(logger); }
		}

		public bool IsWarnEnabled
		{
			get { return isWarnEnabledDelegate(logger); }
		}

		public void Error(object message)
		{
			errorDelegate(logger, message);
		}

		public void Error(object message, Exception exception)
		{
			errorExceptionDelegate(logger,message,exception);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			errorFormatDelegate(logger, format, args);
		}

		public void Fatal(object message)
		{
			fatalDelegate(logger, message);
		}

		public void Fatal(object message, Exception exception)
		{
			fatalExceptionDelegate(logger,message,exception);
		}

		public void Debug(object message)
		{
			debugDelegate(logger, message);
		}

		public void Debug(object message, Exception exception)
		{
			debugExceptionDelegate(logger, message, exception);
		}

		public void DebugFormat(string format, params object[] args)
		{
			debugFormatDelegate(logger, format, args);
		}

		public void Info(object message)
		{
			infoDelegate(logger, message);
		}

		public void Info(object message, Exception exception)
		{
			infoExceptionDelegate(logger, message, exception);
		}

		public void InfoFormat(string format, params object[] args)
		{
			infoFormatDelegate(logger, format, args);
		}

		public void Warn(object message)
		{
			warnDelegate(logger, message);
		}

		public void Warn(object message, Exception exception)
		{
			warnExceptionDelegate(logger, message, exception);
		}

		public void WarnFormat(string format, params object[] args)
		{
			warnFormatDelegate(logger, format, args);
		}
	}
}