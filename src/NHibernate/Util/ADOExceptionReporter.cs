using System;


namespace NHibernate.Util
{
	public class ADOExceptionReporter
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(ADOExceptionReporter));
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

				while (ex != null)
				{
					log.Warn(ex, ex.Message);
					log.Error(ex.Message);
					ex = ex.InnerException;
				}
			}
		}
	}
}
