using System;

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
		/// Convert the given <see cref="System.Data.Common.DbException"/> into custom Exception. 
		/// </summary>
		/// <param name="adoExceptionContextInfo">Available information during exception throw.</param>
		/// <returns> The resulting Exception to throw. </returns>
		Exception Convert(AdoExceptionContextInfo adoExceptionContextInfo);
	}
}