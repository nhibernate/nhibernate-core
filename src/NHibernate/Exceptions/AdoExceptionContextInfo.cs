using System;

namespace NHibernate.Exceptions
{
	/// <summary>
	/// Collect data of an <see cref="ADOException"/> to be converted.
	/// </summary>
	[Serializable]
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

		/// <summary>
		/// Optional EntityName where available in the original exception context.
		/// </summary>
		public string EntityName { get; set; }

		/// <summary>
		/// Optional EntityId where available in the original exception context.
		/// </summary>
		public object EntityId { get; set; }
	}
}