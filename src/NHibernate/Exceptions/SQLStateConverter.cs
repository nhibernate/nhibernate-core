using System;

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// A SQLExceptionConverter implementation which performs no conversion of
	/// the underlying <see cref="System.Data.Common.DbException"/>. 
	/// Interpretation of a SQL error based on <see cref="System.Data.Common.DbException"/>
	/// is not possible as using the ErrorCode (which is, however, vendor-
	/// specific). Use of a ErrorCode-based converter should be preferred approach
	/// for converting/interpreting SQLExceptions. 
	/// </summary>
	public class SQLStateConverter : ISQLExceptionConverter
	{
		public SQLStateConverter(IViolatedConstraintNameExtracter extracter)
		{
		}

		#region ISQLExceptionConverter Members

		public Exception Convert(AdoExceptionContextInfo exceptionInfo)
		{
			/*
			 * So far I know we don't have something similar to "X/Open-compliant SQLState" in .NET
			 * This mean that each Dialect must have its own ISQLExceptionConverter, overriding BuildSQLExceptionConverter method,
			 * and its own IViolatedConstraintNameExtracter if needed.
			 * The System.Data.Common.DbException, of .NET2.0, don't give us something applicable to all dialects.
			 */
			return HandledNonSpecificException(exceptionInfo.SqlException, exceptionInfo.Message, exceptionInfo.Sql);
		}

		#endregion

		/// <summary> Handle an exception not converted to a specific type based on the SQLState. </summary>
		/// <param name="sqlException">The exception to be handled. </param>
		/// <param name="message">An optional message </param>
		/// <param name="sql">Optionally, the sql being performed when the exception occurred. </param>
		/// <returns> The converted exception; should <b>never</b> be null. </returns>
		public static ADOException HandledNonSpecificException(Exception sqlException, string message, string sql)
		{
			return new GenericADOException(message, sqlException, sql);
		}
	}
}
