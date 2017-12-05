using System;

namespace NHibernate
{
	public interface INHibernateLogger
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
		public void Log(InternalLogLevel logLevel, InternalLogValues state, Exception exception)
		{
		}

		public bool IsEnabled(InternalLogLevel logLevel)
		{
			if (logLevel == InternalLogLevel.None) return true;

			return false;
		}
	}

	public struct InternalLogValues
	{
		private readonly string _format;
		private readonly object[] _args;

		/// <summary>
		/// Instantiates a new instance of the <see cref="InternalLogValues"/> structure.
		/// </summary>
		/// <param name="format">a composite format string</param>
		/// <param name="args">An object array that contains zero or more objects to format.  Can be <c>null</c> if there are no values to format.</param>
		public InternalLogValues(string format, object[] args)
		{
			_format = format ?? "[Null]";
			_args = args;
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
