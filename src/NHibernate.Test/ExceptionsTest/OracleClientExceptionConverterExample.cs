using System;
using System.Data.OracleClient;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;

namespace NHibernate.Test.ExceptionsTest
{
	public class OracleClientExceptionConverterExample : ISQLExceptionConverter
	{
		#region ISQLExceptionConverter Members

		public ADOException Convert(Exception sqlException, string message, SqlString sql)
		{
			var sqle = ADOExceptionHelper.ExtractDbException(sqlException) as OracleException;
			if (sqle != null)
			{
				if (sqle.Code == 1036)
				{
					return new ConstraintViolationException(message, sqle.InnerException, sql, null);
				}
				if (sqle.Code == 942)
				{
					return new SQLGrammarException(message, sqle.InnerException, sql);
				}
			}
			return SQLStateConverter.HandledNonSpecificException(sqlException, message, sql);
		}

		#endregion
	}
}