using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace NHibernate.Test
{
	/// <summary>
	/// A disposible object that taps into the "NHibernate.SQL" logger and
	/// collection the log entries being logged.  This class should be used
	/// with a C# using-statement
	/// </summary>
	public class SqlLogSpy : IDisposable
	{
		private Logger sqlLogger;
		Level prevLogLevel;
		private MemoryAppender appender;

		public MemoryAppender Appender
		{
			get { return appender; }
		}

		public SqlLogSpy()
		{
			ILog log = LogManager.GetLogger("NHibernate.SQL");
			sqlLogger = log.Logger as Logger;
			if (sqlLogger == null)
				throw new Exception("Unable to get the SQL logger");

			// Change the log level to DEBUG and temporarily save the previous log level
			prevLogLevel = sqlLogger.Level;
			sqlLogger.Level = Level.Debug;

			// Add a new MemoryAppender to the logger.
			appender = new MemoryAppender();
			sqlLogger.AddAppender(appender);
		}

		public void Dispose()
		{
			// Restore the previous log level of the SQL logger and remove the MemoryAppender
			sqlLogger.Level = prevLogLevel;
			sqlLogger.RemoveAppender(appender);
		}
	}
}
