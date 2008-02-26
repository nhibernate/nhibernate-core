using System;
using NHibernate.SqlCommand;

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// Defines a contract for implementations that know how to convert a <see cref="System.Data.Common.DbException"/>
	/// into NHibernate's <see cref="ADOException"/> hierarchy. 
	/// </summary>
	/// <remarks>
	/// Inspired by Spring's SQLExceptionTranslator.
	/// 
	/// Implementations <b>must</b> have a constructor which takes a
	/// <see cref="IViolatedConstraintNameExtracter"/> parameter.
	/// <para/>
	/// Implementations may implement <see cref="IConfigurable"/> if they need to perform
	/// configuration steps prior to first use.
	/// </remarks>
	/// <seealso cref="SQLExceptionConverterFactory"/>
	public interface ISQLExceptionConverter
	{
		/// <summary> 
		/// Convert the given <see cref="System.Data.Common.DbException"/> into NHibernate's ADOException hierarchy. 
		/// </summary>
		/// <param name="sqlException">The <see cref="System.Data.Common.DbException"/> to be converted. </param>
		/// <param name="message"> An optional error message. </param>
		/// <param name="sql">The SQL that generate the exception</param>
		/// <returns> The resulting ADOException. </returns>
		ADOException Convert(Exception sqlException, string message, SqlString sql);
	}
}