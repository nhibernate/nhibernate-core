using System;
using System.Data.OracleClient;
using NHibernate.Exceptions;

namespace NHibernate.Test.ExceptionsTest
{
	public class OracleClientExceptionConverterExample : ISQLExceptionConverter
	{
		#region ISQLExceptionConverter Members

		public Exception Convert(AdoExceptionContextInfo exInfo)
		{
			var sqle = ADOExceptionHelper.ExtractDbException(exInfo.SqlException) as OracleException;
			if (sqle != null)
			{
				if (sqle.Code == 1036)
				{
					return new ConstraintViolationException(exInfo.Message, sqle.InnerException, exInfo.Sql, null);
				}
				if (sqle.Code == 942)
				{
					return new SQLGrammarException(exInfo.Message, sqle.InnerException, exInfo.Sql);
				}
			}
			return SQLStateConverter.HandledNonSpecificException(exInfo.SqlException, exInfo.Message, exInfo.Sql);
		}

		#endregion
	}
}