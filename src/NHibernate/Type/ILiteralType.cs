using System;

namespace NHibernate.Type {
	/// <summary>
	/// An <see cref="IType"/> that may appear as an SQL literal
	/// </summary>
	public interface ILiteralType {
		
		/// <summary>
		/// When implemented by a class, return a <see cref="String"/> representation 
		/// of the value, suitable for embedding in an SQL statement
		/// </summary>
		/// <param name="value">The object to convert to a string for the SQL statement.</param>
		/// <returns>A string that containts a well formed SQL Statement.</returns>
		string ObjectToSQLString(object value);
	}
}
