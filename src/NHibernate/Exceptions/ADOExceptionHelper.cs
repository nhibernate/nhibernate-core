using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Exceptions
{
	public static class ADOExceptionHelper
	{
		public const string SQLNotAvailable = "SQL not available";

		public static Exception Convert(ISQLExceptionConverter converter, AdoExceptionContextInfo exceptionContextInfo)
		{
			if(exceptionContextInfo == null)
			{
				throw new AssertionFailure("The argument exceptionContextInfo is null.");
			}
			var sql = TryGetActualSqlQuery(exceptionContextInfo.SqlException, exceptionContextInfo.Sql);
			ADOExceptionReporter.LogExceptions(exceptionContextInfo.SqlException,
											   ExtendMessage(exceptionContextInfo.Message, sql, null, null));
			return converter.Convert(exceptionContextInfo);
		}

		/// <summary> 
		/// Converts the given SQLException into Exception hierarchy, as well as performing
		/// appropriate logging. 
		/// </summary>
		/// <param name="converter">The converter to use.</param>
		/// <param name="sqlException">The exception to convert.</param>
		/// <param name="message">An optional error message.</param>
		/// <param name="sql">The SQL executed.</param>
		/// <returns> The converted <see cref="ADOException"/>.</returns>
		public static Exception Convert(ISQLExceptionConverter converter, Exception sqlException, string message,
										   SqlString sql)
		{
			return Convert(converter,
						   new AdoExceptionContextInfo
							{SqlException = sqlException, Message = message, Sql = sql != null ? sql.ToString() : null});
		}

		/// <summary> 
		/// Converts the given SQLException into Exception hierarchy, as well as performing
		/// appropriate logging. 
		/// </summary>
		/// <param name="converter">The converter to use.</param>
		/// <param name="sqlException">The exception to convert.</param>
		/// <param name="message">An optional error message.</param>
		/// <returns> The converted <see cref="ADOException"/>.</returns>
		public static Exception Convert(ISQLExceptionConverter converter, Exception sqlException, string message)
		{
			var sql = TryGetActualSqlQuery(sqlException, SQLNotAvailable);
			return Convert(converter, new AdoExceptionContextInfo {SqlException = sqlException, Message = message, Sql = sql});
		}

		public static Exception Convert(ISQLExceptionConverter converter, Exception sqle, string message, SqlString sql,
										   object[] parameterValues, IDictionary<string, TypedValue> namedParameters)
		{
			sql = TryGetActualSqlQuery(sqle, sql);
			string extendMessage = ExtendMessage(message, sql != null ? sql.ToString() : null, parameterValues, namedParameters);
			ADOExceptionReporter.LogExceptions(sqle, extendMessage);
			return Convert(converter, sqle, extendMessage, sql);
		}

		/// <summary> For the given <see cref="Exception"/>, locates the <see cref="System.Data.Common.DbException"/>. </summary>
		/// <param name="sqlException">The exception from which to extract the <see cref="System.Data.Common.DbException"/> </param>
		/// <returns> The <see cref="System.Data.Common.DbException"/>, or null. </returns>
		public static DbException ExtractDbException(Exception sqlException)
		{
			Exception baseException = sqlException;
			var result = sqlException as DbException;
			while (result == null && baseException != null)
			{
				baseException = baseException.InnerException;
				result = baseException as DbException;
			}
			return result;
		}

		public static string ExtendMessage(string message, string sql, object[] parameterValues,
										   IDictionary<string, TypedValue> namedParameters)
		{
			var sb = new StringBuilder();
			sb.Append(message).AppendLine().Append("[ ").Append(sql ?? SQLNotAvailable).Append(" ]");
			if (parameterValues != null && parameterValues.Length > 0)
			{
				sb.AppendLine().Append("Positional parameters: ");
				for (int index = 0; index < parameterValues.Length; index++)
				{
					object value = parameterValues[index] ?? "null";
					sb.Append(" #").Append(index).Append(">").Append(value);
				}
			}
			if (namedParameters != null && namedParameters.Count > 0)
			{
				sb.AppendLine();
				foreach (var namedParameter in namedParameters)
				{
					object value = namedParameter.Value.Value ?? "null";
					sb.Append("  ").Append("Name:").Append(namedParameter.Key).Append(" - Value:").Append(value);
				}
			}
			sb.AppendLine();
			return sb.ToString();
		}

		public static SqlString TryGetActualSqlQuery(Exception sqle, SqlString sql)
		{
			var query = (string) sqle.Data["actual-sql-query"];
			if (query != null)
			{
				sql = new SqlString(query);
			}
			return sql;
		}

		public static string TryGetActualSqlQuery(Exception sqle, string sql)
		{
			var query = (string)sqle.Data["actual-sql-query"];
			return query ?? sql;
		}
	}
}