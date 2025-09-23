using System;
using NHibernate.Exceptions;

namespace NHibernate.Test.ExceptionsTest
{
	public class OracleClientExceptionConverterExample : ISQLExceptionConverter
	{
		#region ISQLExceptionConverter Members

		public Exception Convert(AdoExceptionContextInfo exInfo)
		{
			var sqle = ADOExceptionHelper.ExtractDbException(exInfo.SqlException);
			if (sqle != null && sqle.GetType().Name == "OracleException")
			{
				var code = (int)sqle.GetType().GetProperty("Code").GetValue(sqle);
				if (code == 1036)
				{
					return new ConstraintViolationException(exInfo.Message, sqle.InnerException, exInfo.Sql, null);
				}
				if (code == 942)
				{
					return new SQLGrammarException(exInfo.Message, sqle.InnerException, exInfo.Sql);
				}
			}
			return SQLStateConverter.HandledNonSpecificException(exInfo.SqlException, exInfo.Message, exInfo.Sql);
		}

		#endregion
	}
}
