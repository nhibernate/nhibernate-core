using System;

namespace NHibernate
{
	/// <summary>
	/// Extensions method for logging.
	/// </summary>
	public static class NHibernateLoggerExtensions
	{
		public static bool IsDebugEnabled(this INHibernateLogger logger) => logger.IsEnabled(NHibernateLogLevel.Debug);
		public static bool IsInfoEnabled(this INHibernateLogger logger) => logger.IsEnabled(NHibernateLogLevel.Info);
		public static bool IsWarnEnabled(this INHibernateLogger logger) => logger.IsEnabled(NHibernateLogLevel.Warn);
		public static bool IsErrorEnabled(this INHibernateLogger logger) => logger.IsEnabled(NHibernateLogLevel.Error);
		public static bool IsFatalEnabled(this INHibernateLogger logger) => logger.IsEnabled(NHibernateLogLevel.Fatal);

		public static void Fatal(this INHibernateLogger logger, Exception exception, string format, params object[] args)
		{
			logger.Log(NHibernateLogLevel.Fatal, new NHibernateLogValues(format, args), exception);
		}

		public static void Fatal(this INHibernateLogger logger, Exception exception, string format)
		{
			logger.Log(NHibernateLogLevel.Fatal, new NHibernateLogValues(format, null), exception);
		}

		public static void Fatal(this INHibernateLogger logger, string format, params object[] args)
		{
			logger.Log(NHibernateLogLevel.Fatal, new NHibernateLogValues(format, args), null);
		}

		public static void Fatal(this INHibernateLogger logger, string format)
		{
			logger.Log(NHibernateLogLevel.Fatal, new NHibernateLogValues(format, null), null);
		}

		public static void Error(this INHibernateLogger logger, Exception exception, string format, params object[] args)
		{
			logger.Log(NHibernateLogLevel.Error, new NHibernateLogValues(format, args), exception);
		}

		public static void Error(this INHibernateLogger logger, Exception exception, string format)
		{
			logger.Log(NHibernateLogLevel.Error, new NHibernateLogValues(format, null), exception);
		}

		public static void Error(this INHibernateLogger logger, string format, params object[] args)
		{
			logger.Log(NHibernateLogLevel.Error, new NHibernateLogValues(format, args), null);
		}

		public static void Error(this INHibernateLogger logger, string format)
		{
			logger.Log(NHibernateLogLevel.Error, new NHibernateLogValues(format, null), null);
		}

		public static void Warn(this INHibernateLogger logger, Exception exception, string format, params object[] args)
		{
			logger.Log(NHibernateLogLevel.Warn, new NHibernateLogValues(format, args), exception);
		}

		public static void Warn(this INHibernateLogger logger, Exception exception, string format)
		{
			logger.Log(NHibernateLogLevel.Warn, new NHibernateLogValues(format, null), exception);
		}

		public static void Warn(this INHibernateLogger logger, string format, params object[] args)
		{
			logger.Log(NHibernateLogLevel.Warn, new NHibernateLogValues(format, args), null);
		}

		public static void Warn(this INHibernateLogger logger, string format)
		{
			logger.Log(NHibernateLogLevel.Warn, new NHibernateLogValues(format, null), null);
		}

		public static void Info(this INHibernateLogger logger, Exception exception, string format, params object[] args)
		{
			logger.Log(NHibernateLogLevel.Info, new NHibernateLogValues(format, args), exception);
		}

		public static void Info(this INHibernateLogger logger, Exception exception, string format)
		{
			logger.Log(NHibernateLogLevel.Info, new NHibernateLogValues(format, null), exception);
		}

		public static void Info(this INHibernateLogger logger, string format, params object[] args)
		{
			logger.Log(NHibernateLogLevel.Info, new NHibernateLogValues(format, args), null);
		}

		public static void Info(this INHibernateLogger logger, string format)
		{
			logger.Log(NHibernateLogLevel.Info, new NHibernateLogValues(format, null), null);
		}

		public static void Debug(this INHibernateLogger logger, Exception exception, string format, params object[] args)
		{
			logger.Log(NHibernateLogLevel.Debug, new NHibernateLogValues(format, args), exception);
		}

		public static void Debug(this INHibernateLogger logger, Exception exception, string format)
		{
			logger.Log(NHibernateLogLevel.Debug, new NHibernateLogValues(format, null), exception);
		}

		public static void Debug(this INHibernateLogger logger, string format, params object[] args)
		{
			logger.Log(NHibernateLogLevel.Debug, new NHibernateLogValues(format, args), null);
		}

		public static void Debug(this INHibernateLogger logger, string format)
		{
			logger.Log(NHibernateLogLevel.Debug, new NHibernateLogValues(format, null), null);
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
}
