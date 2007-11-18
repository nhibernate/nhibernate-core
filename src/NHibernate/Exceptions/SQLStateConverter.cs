using System;
using NHibernate.SqlCommand;

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// A SQLExceptionConverter implementation which performs converion based on
	/// the underlying SQLState. Interpretation of a SQL error based on SQLState
	/// is not nearly as accurate as using the ErrorCode (which is, however, vendor-
	/// specific).  Use of a ErrorCode-based converter should be preferred approach
	/// for converting/interpreting SQLExceptions. 
	/// </summary>
	public class SQLStateConverter : ISQLExceptionConverter
	{
		private readonly IViolatedConstraintNameExtracter extracter;

		public SQLStateConverter(IViolatedConstraintNameExtracter extracter)
		{
			this.extracter = extracter;
		}

		#region ISQLExceptionConverter Members

		public ADOException Convert(Exception sqlException, string message, SqlString sql)
		{
			throw new NotImplementedException();
		}

		#endregion

		/// <summary> Handle an exception not converted to a specific type based on the SQLState.
		/// 
		/// </summary>
		/// <param name="sqlException">The exception to be handled.
		/// </param>
		/// <param name="message">     An optional message
		/// </param>
		/// <param name="sql">         Optionally, the sql being performed when the exception occurred.
		/// </param>
		/// <returns> The converted exception; should <b>never</b> be null.
		/// </returns>
		protected internal virtual ADOException HandledNonSpecificException(Exception sqlException, string message, SqlString sql)
		{
			return new GenericADOException(message, sqlException, sql);
		}
	}
}