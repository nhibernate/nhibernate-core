using System;
using NHibernate.SqlCommand;

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// Defines a contract for implementations that know how to convert a SQLException
	/// into Hibernate's JDBCException hierarchy. 
	/// </summary>
	/// <remarks>
	/// Inspired by Spring's SQLExceptionTranslator.
	/// 
	/// Implementations <b>must</b> have a constructor which takes a
	/// {@link ViolatedConstraintNameExtracter} parameter.
	/// <p/>
	/// Implementations may implement {@link Configurable} if they need to perform
	/// configuration steps prior to first use.
	/// </remarks>
	public interface ISQLExceptionConverter
	{
		/// <summary> 
		/// Convert the given SQLException into Hibernate's JDBCException hierarchy. 
		/// </summary>
		/// <param name="sqlException">The SQLException to be converted. </param>
		/// <param name="message"> An optional error message. </param>
		/// <param name="sql">The SQL that generate the exception</param>
		/// <returns> The resulting JDBCException. </returns>
		ADOException Convert(Exception sqlException, string message, SqlString sql);
	}
}