using System;

namespace NHibernate.Exceptions
{
	/// <summary>
	/// Collect data of an <see cref="ADOException"/> to be converted.
	/// </summary>
	public class AdoExceptionContextInfo
	{
		// This class was introduced, in NH, to allow less intrusive possible extensions 
		// of information given to the ISQLExceptionConverter 
		// (extensions of a class instead succesive changes of a method)

		/// <summary>
		/// The <see cref="System.Data.Common.DbException"/> to be converted.
		/// </summary>
		public Exception SqlException { get; set; }

		/// <summary>
		/// An optional error message.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// The SQL that generate the exception
		/// </summary>
		public string Sql { get; set; }
	}

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