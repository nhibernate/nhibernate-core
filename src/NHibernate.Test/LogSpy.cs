using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace NHibernate.Test
{
	public class LogSpy : IDisposable
	{
		private readonly MemoryAppender appender;
		private readonly Logger logger;
		private readonly Level prevLogLevel;

		public LogSpy(ILog log, Level level)
		{
			logger = log.Logger as Logger;
			if (logger == null)
			{
				throw new Exception("Unable to get the logger");
			}

			// Change the log level to DEBUG and temporarily save the previous log level
			prevLogLevel = logger.Level;
			logger.Level = level;

			// Add a new MemoryAppender to the logger.
			appender = new MemoryAppender();
			logger.AddAppender(appender);
		}

		public LogSpy(ILog log, bool disable)
			: this(log, disable ? Level.Off : Level.Debug)
		{
		}

		public LogSpy(ILog log) : this(log, false) { }
		public LogSpy(System.Type loggerType) : this(LogManager.GetLogger(loggerType), false) { }
		public LogSpy(System.Type loggerType, bool disable) : this(LogManager.GetLogger(loggerType), disable) { }

		public LogSpy(string loggerName) : this(LogManager.GetLogger(typeof(LogSpy).Assembly, loggerName), false) { }
		public LogSpy(string loggerName, bool disable) : this(LogManager.GetLogger(typeof(LogSpy).Assembly, loggerName), disable) { }

		public MemoryAppender Appender
		{
			get { return appender; }
		}

		public virtual string GetWholeLog()
		{
			var wholeMessage = new StringBuilder();
			foreach (LoggingEvent loggingEvent in Appender.GetEvents())
			{
				wholeMessage
					.Append(loggingEvent.LoggerName)
					.Append(" ")
					.Append(loggingEvent.RenderedMessage)
					.AppendLine();
			}
			return wholeMessage.ToString();
		}

		#region IDisposable Members

		public void Dispose()
		{
			// Restore the previous log level of the SQL logger and remove the MemoryAppender
			logger.Level = prevLogLevel;
			logger.RemoveAppender(appender);
		}

		#endregion
	}
}
