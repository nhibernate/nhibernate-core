using System;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Superclass of <see cref="ValueType"/> types.
	/// </summary>
	[Serializable]
	public abstract class PrimitiveType : ImmutableType, ILiteralType
	{
		/// <summary>
		/// Initialize a new instance of the PrimitiveType class using a <see cref="SqlType"/>. 
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="SqlType"/>.</param>
		protected PrimitiveType(SqlType sqlType)
			: base(sqlType) {}

		public abstract System.Type PrimitiveClass { get;}

		public abstract object DefaultValue { get;}

		#region ILiteralType Members

		/// <summary>
		/// When implemented by a class, return a <see cref="String"/> representation 
		/// of the value, suitable for embedding in an SQL statement
		/// </summary>
		/// <param name="value">The object to convert to a string for the SQL statement.</param>
		/// <param name="dialect"></param>
		/// <returns>A string that containts a well formed SQL Statement.</returns>
		public abstract string ObjectToSQLString(object value, Dialect.Dialect dialect);

		#endregion

		/// <summary>
		/// A representation of the value to be embedded in an XML element 
		/// </summary>
		/// <param name="val">The object that contains the values.
		/// </param>
		/// <returns>An Xml formatted string.</returns>
		/// <remarks>
		/// This just calls <see cref="Object.ToString"/> so if there is 
		/// a possibility of this PrimitiveType having any characters
		/// that need to be encoded then this method should be overridden.
		/// </remarks>
		// TODO: figure out if this is used to build Xml strings or will have encoding done automatically.
		public override string ToString(object val)
		{
			return val.ToString();
		}
	}
}
