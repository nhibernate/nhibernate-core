using System;

namespace NHibernate.Type {
	/// <summary>
	/// A type that may appear as an SQL literal
	/// </summary>
	public interface ILiteralType {
		
		/// <summary>
		/// String representation of the value, suitable for embedding in an SQL statement
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		string ObjectToSQLString(object value);
	}
}
