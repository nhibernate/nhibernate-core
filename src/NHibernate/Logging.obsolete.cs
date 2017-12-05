using System;
using System.Configuration;
using System.IO;
using System.Linq;

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
				throw new InstantiationException(loggerFactoryType + "Type does not implement " + typeof(INHibernateLoggerFactory) + " or " + typeof(ILoggerFactory), ex, loggerFactoryType);
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
					nhibernateLoggerClass = typeof(Log4NetLoggerFactory).AssemblyQualifiedName;
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

	// Since 5.1
	[Obsolete("To set no-logging, use NHibernateLogger.SetLoggersFactory(default(INHibernateLoggerFactory))")]
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
	[Obsolete("To set no-logging, use NHibernateLogger.SetLoggersFactory(default(INHibernateLoggerFactory))")]
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
}
