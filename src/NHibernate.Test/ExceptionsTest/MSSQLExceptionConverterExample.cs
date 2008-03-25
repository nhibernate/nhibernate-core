using System;
using System.Data.SqlClient;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;

namespace NHibernate.Test.ExceptionsTest
{
	// This is an example on how to categorize SQL exceptions
	// It is only to pass the test
	public class MSSQLExceptionConverterExample : ISQLExceptionConverter
	{
		#region ISQLExceptionConverter Members

		public ADOException Convert(Exception sqlException, string message, SqlString sql)
		{
			SqlException sqle = ADOExceptionHelper.ExtractDbException(sqlException) as SqlException;
			if(sqle != null)
			{
				if (sqle.Number == 547)
					return new ConstraintViolationException(message, sqle.InnerException, sql, null);
				if (sqle.Number == 208)
					return new SQLGrammarException(message, sqle.InnerException, sql);
			}
			return SQLStateConverter.HandledNonSpecificException(sqlException, message, sql);
		}

		#endregion
	}
}
