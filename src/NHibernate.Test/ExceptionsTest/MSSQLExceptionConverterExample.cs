using System;
using System.Data.SqlClient;
using NHibernate.Exceptions;

namespace NHibernate.Test.ExceptionsTest
{
	// This is an example on how to categorize SQL exceptions
	// It is only to pass the test
	public class MSSQLExceptionConverterExample : ISQLExceptionConverter
	{
		#region ISQLExceptionConverter Members

		public Exception Convert(AdoExceptionContextInfo exInfo)
		{
			var dbEx = ADOExceptionHelper.ExtractDbException(exInfo.SqlException);
			if (dbEx is SqlException sqle)
			{
				if (sqle.Number == 547)
					return new ConstraintViolationException(exInfo.Message, sqle.InnerException, exInfo.Sql, null);
				if (sqle.Number == 208)
					return new SQLGrammarException(exInfo.Message, sqle.InnerException, exInfo.Sql);
			}

			if (dbEx is Microsoft.Data.SqlClient.SqlException msSqle)
			{
				if (msSqle.Number == 547)
					return new ConstraintViolationException(exInfo.Message, msSqle.InnerException, exInfo.Sql, null);
				if (msSqle.Number == 208)
					return new SQLGrammarException(exInfo.Message, msSqle.InnerException, exInfo.Sql);
			}
			return SQLStateConverter.HandledNonSpecificException(exInfo.SqlException, exInfo.Message, exInfo.Sql);
		}

		#endregion
	}
}
