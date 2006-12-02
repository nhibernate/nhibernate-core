using System;
using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Exceptions
{
	public sealed class ADOExceptionHelper
	{
		private ADOExceptionHelper()
		{
		}

		/// <summary>
		/// Converts the given SQLException into NHibernate's ADOException hierarchy, as well as performing
		/// appropriate logging.
		/// </summary>
		/// <!--<param name="converter">The converter to use.</param>-->
		/// <param name="sqlException">The exception to convert.</param>
		/// <param name="message">An optional error message.</param>
		/// <returns>The converted ADOException.</returns>
		public static ADOException Convert( /*ISQLExceptionConverter converter,*/ Exception sqlException, string message)
		{
			ADOExceptionReporter.LogExceptions(sqlException, message);
			// return converter.Convert( sqlException, message );
			return new ADOException(message, sqlException);
		}

		public static ADOException Convert(Exception sqlException, string message, SqlString sql)
		{
			ADOExceptionReporter.LogExceptions(sqlException, message + " [" + sql + "]");
			return new ADOException(message, sqlException, sql);
		}


		public static ADOException Convert(Exception sqle, string message, SqlString sql, object[] parameterValues, IDictionary namedParameters)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(message).Append(Environment.NewLine).
				Append("[ ").Append(sql).Append(" ]")
				.Append(Environment.NewLine);
			if (parameterValues.Length > 0)
			{
				sb.Append("Positinal Parameters: ");
				int index = 0;
				foreach (object parameterValue in parameterValues)
				{
					object value = parameterValue;
					if (value == null)
						value = "null";
					sb.Append("  ").Append(index).Append(" ").Append(value).Append(Environment.NewLine);
				}
			}
			if (namedParameters.Count > 0)
			{
				foreach (DictionaryEntry namedParameter in namedParameters)
				{
					object value = namedParameter.Value;
					if (value == null)
						value = "null";
					sb.Append("  ").Append("Name: ").Append(namedParameter.Key)
						.Append(" - Value: ").Append(value).Append(Environment.NewLine);
				}
			}
			ADOExceptionReporter.LogExceptions(sqle, sb.ToString());
			return new ADOException(sb.ToString(), sqle, sql);
		}
	}
}
