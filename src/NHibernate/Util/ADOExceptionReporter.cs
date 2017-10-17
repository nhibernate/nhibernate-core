using System;


namespace NHibernate.Util
{
	public class ADOExceptionReporter
	{
		private static readonly IInternalLogger2 log = LoggerProvider.LoggerFor(typeof(ADOExceptionReporter));
		public const string DefaultExceptionMsg = "SQL Exception";

		private ADOExceptionReporter()
		{
		}

		public static void LogExceptions(Exception ex)
		{
			LogExceptions(ex, null);
		}

		public static void LogExceptions(Exception ex, string message)
		{
			if (log.IsErrorEnabled())
			{
				if (log.IsDebugEnabled())
				{
					message = StringHelper.IsNotEmpty(message) ? message : DefaultExceptionMsg;
					log.Debug(ex, message);
				}

				// Pass full exception on highest call
				if (log.IsWarnEnabled()) log.Warn(ex, ex.ToString());
				log.Error(ex, ex.Message);
				ex = ex.InnerException;
				while (ex != null)
				{
					if (log.IsWarnEnabled()) log.Warn(ex.ToString());
					log.Error(ex.Message);
					ex = ex.InnerException;
				}
			}
		}
	}
}
