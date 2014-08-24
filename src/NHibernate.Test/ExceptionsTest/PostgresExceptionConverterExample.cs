using System;
using System.Data.Common;
using NHibernate.Exceptions;

namespace NHibernate.Test.ExceptionsTest
{
	public class PostgresExceptionConverterExample : ISQLExceptionConverter
	{
		#region ISQLExceptionConverter Members

		public Exception Convert(AdoExceptionContextInfo exInfo)
		{
			var sqle = ADOExceptionHelper.ExtractDbException(exInfo.SqlException) as DbException;
			if (sqle != null)
			{
				string code = (string)sqle.GetType().GetProperty("Code").GetValue(sqle, null);

				if (code == "23503")
				{
					return new ConstraintViolationException(exInfo.Message, sqle.InnerException, exInfo.Sql, null);
				}
				if (code == "42P01")
				{
					return new SQLGrammarException(exInfo.Message, sqle.InnerException, exInfo.Sql);
				}
			}
			return SQLStateConverter.HandledNonSpecificException(exInfo.SqlException, exInfo.Message, exInfo.Sql);
		}

		#endregion
	}
}