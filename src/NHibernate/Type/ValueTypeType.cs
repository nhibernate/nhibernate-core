using System;

using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type 
{
	/// <summary>
	/// Superclass of <see cref="ValueType"/> types.
	/// </summary>
	public abstract class ValueTypeType : ImmutableType, ILiteralType 
	{
		
		/// <summary>
		/// Initialize a new instance of the ValueTypeType class using a 
		/// <see cref="SqlType"/>. 
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="SqlType"/>.</param>
		protected ValueTypeType(SqlType sqlType) : base(sqlType) 
		{
		}
	
		/// <summary>
		/// Compare two instances of the class mapped by this 
		/// IType for persistence "equality" - ie. Equality of persistent state.
		/// </summary>
		/// <param name="x">The left hand side object.</param>
		/// <param name="y">The right hand side object.</param>
		/// <returns>True if the two objects contain the same values.</returns>
		public override bool Equals(object x, object y) 
		{
			return ObjectUtils.Equals(x, y);
		}
	
		/// <summary>
		/// A representation of the value to be embedded in an XML element 
		/// </summary>
		/// <param name="val">The object that contains the values.
		/// </param>
		/// <returns>An Xml formatted string.</returns>
		/// <remarks>
		/// This just calls <see cref="Object.ToString"/> so if there is 
		/// a possibility of this <see cref="PrimitiveType"/> having any characters
		/// that need to be encoded then this method should be overridden.
		/// 
		/// TODO: figure out if this is used to build Xml strings or will have encoding
		/// done automattically.
		/// </remarks>
		public override string ToXML(object val) 
		{
			return val.ToString();
		}

		/// <summary>
		/// When implemented by a class, return a <see cref="String"/> representation 
		/// of the value, suitable for embedding in an SQL statement
		/// </summary>
		/// <param name="val">The object to convert to a string for the SQL statement.</param>
		/// <returns>A string that containts a well formed SQL Statement.</returns>
		public abstract string ObjectToSQLString(object val);
	}
}