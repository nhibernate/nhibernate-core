using System;
using System.Data.SqlClient;
using NHibernate.Exceptions;

namespace NHibernate.Test.NHSpecificTest.NH1553.MsSQL
{
	/// <summary>
	/// Exception converter to convert SQL Snapshot isolation update conflict to 
	/// StaleObjectStateException
	/// </summary>
	public class SQLUpdateConflictToStaleStateExceptionConverter : ISQLExceptionConverter
	{
		public Exception Convert(AdoExceptionContextInfo exInfo)
		{
			var sqle = ADOExceptionHelper.ExtractDbException(exInfo.SqlException) as SqlException;
			if ((sqle != null) && (sqle.Number == 3960))
			{
				return new StaleObjectStateException(exInfo.EntityName, exInfo.EntityId);
			}
			return SQLStateConverter.HandledNonSpecificException(exInfo.SqlException, exInfo.Message, exInfo.Sql);
		}
	}
}