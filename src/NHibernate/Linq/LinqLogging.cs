using System.Linq.Expressions;

namespace NHibernate.Linq
{
	internal static class LinqLogging
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor("NHibernate.Linq");

		/// <summary>
		/// If debug logging is enabled, log a string such as "msg: expression.ToString()".
		/// </summary>
		internal static void LogExpression(string msg, Expression expression)
		{
			if (Log.IsDebugEnabled)
				Log.DebugFormat("{0}: {1}", msg, expression.ToString());
		}
	}
}