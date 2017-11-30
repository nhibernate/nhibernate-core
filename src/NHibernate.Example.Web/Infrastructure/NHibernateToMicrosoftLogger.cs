using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;

namespace NHibernate.Example.Web.Infrastructure
{
	public class NHibernateToMicrosoftLogger : INHibernateLogger
	{
		private readonly ILogger _msLogger;

		public NHibernateToMicrosoftLogger(ILogger msLogger)
		{
			_msLogger = msLogger ?? throw new ArgumentNullException(nameof(msLogger));
		}

		private static readonly Dictionary<InternalLogLevel, LogLevel> MapLevels = new Dictionary<InternalLogLevel, LogLevel>
		{
			{ InternalLogLevel.Trace, LogLevel.Trace },
			{ InternalLogLevel.Debug, LogLevel.Debug },
			{ InternalLogLevel.Warn, LogLevel.Warning },
			{ InternalLogLevel.Error, LogLevel.Error },
			{ InternalLogLevel.Fatal, LogLevel.Critical },
			{ InternalLogLevel.None, LogLevel.None },
		};

		public void Log(InternalLogLevel logLevel, InternalLogValues state, Exception exception)
		{
			_msLogger.Log(MapLevels[logLevel], 0, new FormattedLogValues(state.Format, state.Args), exception, MessageFormatter);
		}

		public bool IsEnabled(InternalLogLevel logLevel)
		{
			return _msLogger.IsEnabled(MapLevels[logLevel]);
		}

		private static string MessageFormatter(object state, Exception error)
		{
			return state.ToString();
		}
	}
}
