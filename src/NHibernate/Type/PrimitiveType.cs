using System;

using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type {

	/// <summary>
	/// Superclass of primitive / primitive wrapper types.
	/// </summary>
	/// <remarks>
	/// <para>
	/// DOTNET has no concept of primitive types like Java does.  In Java there is
	/// a primitive type of "int" and an object type of java.lang.Integer.  In DOTNET
	/// there is just a struct System.Int32
	/// </para>
	/// <para>
	/// I don't know if putting the Structs under PrimitiveType of MutableType would
	/// be a better option.
	/// </para>
	/// </remarks>
	public abstract class PrimitiveType : ImmutableType, ILiteralType {
		
		/// <summary>
		/// Initialize a new instance of the PrimitiveType class using a 
		/// <see cref="SqlType"/>. 
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="SqlType"/>.</param>
		protected PrimitiveType(SqlType sqlType) : base(sqlType) 
		{
		}

		/// <summary>
		/// When implemented by a class, gets the <see cref="System.Type"/> wrapped
		/// by this <see cref="PrimitiveType"/>
		/// </summary>
		/// <value>
		/// The <see cref="System.Type"/> from the .NET framework.
		/// </value>
		/// <remarks>
		/// This is used to establish the class of an array of this <see cref="PrimitiveType"/>
		/// in the Binder and HQL.
		/// </remarks>
		public abstract System.Type PrimitiveClass { get; }
	
		/// <summary>
		/// Compare two instances of the class mapped by this 
		/// IType for persistence "equality" - ie. Equality of persistent state.
		/// </summary>
		/// <param name="x">The left hand side object.</param>
		/// <param name="y">The right hand side object.</param>
		/// <returns>True if the two objects contain the same values.</returns>
		public override bool Equals(object x, object y) {
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