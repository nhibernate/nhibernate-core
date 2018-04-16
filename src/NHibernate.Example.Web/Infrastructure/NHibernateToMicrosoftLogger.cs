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

		private static readonly Dictionary<NHibernateLogLevel, LogLevel> MapLevels = new Dictionary<NHibernateLogLevel, LogLevel>
		{
			{ NHibernateLogLevel.Trace, LogLevel.Trace },
			{ NHibernateLogLevel.Debug, LogLevel.Debug },
			{ NHibernateLogLevel.Info, LogLevel.Information },
			{ NHibernateLogLevel.Warn, LogLevel.Warning },
			{ NHibernateLogLevel.Error, LogLevel.Error },
			{ NHibernateLogLevel.Fatal, LogLevel.Critical },
			{ NHibernateLogLevel.None, LogLevel.None },
		};

		public void Log(NHibernateLogLevel logLevel, NHibernateLogValues state, Exception exception)
		{
			_msLogger.Log(MapLevels[logLevel], 0, new FormattedLogValues(state.Format, state.Args), exception, MessageFormatter);
		}

		public bool IsEnabled(NHibernateLogLevel logLevel)
		{
			return _msLogger.IsEnabled(MapLevels[logLevel]);
		}

		private static string MessageFormatter(object state, Exception error)
		{
			return state.ToString();
		}
	}
}
