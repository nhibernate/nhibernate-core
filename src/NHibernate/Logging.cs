using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace NHibernate
{
	public interface INHibernateLogger
	{
		/// <summary>Writes a log entry.</summary>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="state">The entry to be written.</param>
		/// <param name="exception">The exception related to this entry.</param>
		void Log(NHibernateLogLevel logLevel, NHibernateLogValues state, Exception exception);

		/// <summary>
		/// Checks if the given <paramref name="logLevel" /> is enabled.
		/// </summary>
		/// <param name="logLevel">level to be checked.</param>
		/// <returns><c>true</c> if enabled.</returns>
		bool IsEnabled(NHibernateLogLevel logLevel);
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
	public static class NHibernateLogger
	{
		private const string nhibernateLoggerConfKey = "nhibernate-logger";
		private static INHibernateLoggerFactory _loggerFactory;

#pragma warning disable 618
		internal static ILoggerFactory LegacyLoggerFactory { get; private set; }
#pragma warning restore 618

		static NHibernateLogger()
		{
			var nhibernateLoggerClass = GetNhibernateLoggerClass();
			var loggerFactory = string.IsNullOrEmpty(nhibernateLoggerClass) ? null : GetLoggerFactory(nhibernateLoggerClass);
			SetLoggersFactory(loggerFactory);
		}

		/// <summary>
		/// Specify the logger factory to use for building loggers.
		/// </summary>
		/// <param name="loggerFactory">A logger factory.</param>
		public static void SetLoggersFactory(INHibernateLoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory ?? new NoLoggingNHibernateLoggerFactory();

#pragma warning disable 618
			// Also keep global state for obsolete logger
			if (loggerFactory == null)
			{
				LegacyLoggerFactory = new NoLoggingLoggerFactory();
			}
			else
			{
				if (loggerFactory is LoggerProvider.LegacyLoggerFactoryAdaptor legacyAdaptor)
				{
					LegacyLoggerFactory = legacyAdaptor.Factory;
				}
				else
				{
					LegacyLoggerFactory = new LoggerProvider.ReverseLegacyLoggerFactoryAdaptor(loggerFactory);
				}
			}
#pragma warning restore 618
		}

		/// <summary>
		/// Get a logger for the given log key.
		/// </summary>
		/// <param name="keyName">The log key.</param>
		/// <returns>A NHibernate logger.</returns>
		public static INHibernateLogger For(string keyName)
		{
			return _loggerFactory.LoggerFor(keyName);
		}

		/// <summary>
		/// Get a logger using the given type as log key.
		/// </summary>
		/// <param name="type">The type to use as log key.</param>
		/// <returns>A NHibernate logger.</returns>
		public static INHibernateLogger For(System.Type type)
		{
			return _loggerFactory.LoggerFor(type);
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
					loggerFactory = new LoggerProvider.LegacyLoggerFactoryAdaptor(oldStyleFactory);
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
		public void Log(NHibernateLogLevel logLevel, NHibernateLogValues state, Exception exception)
		{
		}

		public bool IsEnabled(NHibernateLogLevel logLevel)
		{
			if (logLevel == NHibernateLogLevel.None) return true;

			return false;
		}
	}

	public struct NHibernateLogValues
	{
		private readonly string _format;
		private readonly object[] _args;

		/// <summary>
		/// Instantiates a new instance of the <see cref="NHibernateLogValues"/> structure.
		/// </summary>
		/// <param name="format">A composite format string</param>
		/// <param name="args">An object array that contains zero or more objects to format.  Can be <c>null</c> if there are no values to format.</param>
		public NHibernateLogValues(string format, object[] args)
		{
			_format = format ?? "[Null]";
			_args = args ?? Array.Empty<object>();
		}

		/// <summary>
		/// Returns the composite format string.
		/// </summary>
		/// <remarks>
		/// A composite format string consists of zero or more runs of fixed text intermixed with
		/// one or more format items, which are indicated by an index number delimited with brackets
		/// (for example, {0}). The index of each format item corresponds to an argument in an object
		/// list that follows the composite format string.
		/// </remarks>
		public string Format => _format;

		/// <summary>
		/// An object array that contains zero or more objects to format.  Can be <c>null</c> if there are no values to format.
		/// </summary>
		public object[] Args => _args;

		/// <summary>
		/// Returns the string that results from formatting the composite format string along with
		/// its arguments by using the formatting conventions of the current culture.
		/// </summary>
		public override string ToString()
		{
			return _args?.Length > 0 ? string.Format(_format, _args) : Format;
		}
	}

	/// <summary>Defines logging severity levels.</summary>
	public enum NHibernateLogLevel
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
