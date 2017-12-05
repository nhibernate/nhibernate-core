using System;

namespace NHibernate
{
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
}