using System;
using System.Data.Common;
using NHibernate;
using NHibernate.Exceptions;
using NHibernate.SqlCommand;

public class PostgresExceptionConverterExample : ISQLExceptionConverter
{
	#region ISQLExceptionConverter Members

	public ADOException Convert(Exception sqlException, string message, SqlString sql)
	{
		var sqle = ADOExceptionHelper.ExtractDbException(sqlException) as DbException;
		if (sqle != null)
		{
			string code = (string) sqle.GetType().GetProperty("Code").GetValue(sqle, null);

			if (code == "23503")
			{
				return new ConstraintViolationException(message, sqle.InnerException, sql, null);
			}
			if (code == "42P01")
			{
				return new SQLGrammarException(message, sqle.InnerException, sql);
			}
		}
		return SQLStateConverter.HandledNonSpecificException(sqlException, message, sql);
	}

	#endregion
}