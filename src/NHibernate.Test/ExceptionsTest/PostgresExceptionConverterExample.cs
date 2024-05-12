using System;
using NHibernate.Exceptions;
using Npgsql;

namespace NHibernate.Test.ExceptionsTest
{
	public class PostgresExceptionConverterExample : ISQLExceptionConverter
	{
		#region ISQLExceptionConverter Members

		public Exception Convert(AdoExceptionContextInfo exInfo)
		{
			if (ADOExceptionHelper.ExtractDbException(exInfo.SqlException) is PostgresException pge)
			{
				string code = pge.SqlState;
				if (code == "23503")
				{
					return new ConstraintViolationException(exInfo.Message, pge.InnerException, exInfo.Sql, null);
				}

				if (code == "42P01")
				{
					return new SQLGrammarException(exInfo.Message, pge.InnerException, exInfo.Sql);
				}
			}

			return SQLStateConverter.HandledNonSpecificException(exInfo.SqlException, exInfo.Message, exInfo.Sql);
		}

		#endregion
	}
}
