using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using NHibernate.Exceptions;

namespace NHibernate.Test.ExceptionsTest
{
	public class FbExceptionConverterExample : ISQLExceptionConverter
	{
		#region ISQLExceptionConverter Members

		public Exception Convert(AdoExceptionContextInfo adoExceptionContextInfo)
		{
			var sqle = ADOExceptionHelper.ExtractDbException(adoExceptionContextInfo.SqlException) as DbException;
			if (sqle != null)
			{
				if (sqle.ErrorCode == 335544466)
				{
					return new ConstraintViolationException(adoExceptionContextInfo.Message, sqle.InnerException, adoExceptionContextInfo.Sql, null);
				}
				if (sqle.ErrorCode == 335544569)
				{
					return new SQLGrammarException(adoExceptionContextInfo.Message, sqle.InnerException, adoExceptionContextInfo.Sql);
				}
			}
			return SQLStateConverter.HandledNonSpecificException(adoExceptionContextInfo.SqlException, adoExceptionContextInfo.Message, adoExceptionContextInfo.Sql);
		}

		#endregion
	}
}
