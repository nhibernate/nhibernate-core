using System;

using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Superclass for mutable nullable types.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This might be the point where we branch out from Hibernate's design and move 
	/// to a StructType and an ObjectType because these follow along the lines of a 
	/// Mutable/Immutable Type design.
	/// </para>
	/// <para>
	/// I still need to think about that though...
	/// </para>
	/// <para>
	/// The only types that are Mutable are <see cref="BinaryType"/>, 
	/// <see cref="DateTimeType"/>, <see cref="SerializableType"/>, and
	/// <see cref="TimestampType"/>.  I don't know what makes them mutable
	/// but I am pretty sure that <see cref="BinaryType"/> and <see cref="SerializableType"/>
	/// don't implement a nice equals because they are based on System.Byte[] array.  In 
	/// DOTNET arrays don't implement a nice equals.  Arrays don't override 
	/// Object.Equals(Object) so they inherit Objects implementation - which is reference
	/// based.
	/// </para>
	/// <para>
	/// I don't know what about the <see cref="DateTimeType"/> and <see cref="TimestampType"/>
	/// makes them mutable in Java.  It might have something to do with milliseconds and ticks
	/// and how some databases store the values.
	/// </para>
	/// </remarks>
	public abstract class MutableType : NullableType 
	{
		
		
		/// <summary>
		/// Initialize a new instance of the MutableType class using a 
		/// <see cref="SqlType"/>. 
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="SqlType"/>.</param>
		protected MutableType(SqlType sqlType) : base(sqlType) 
		{
		}

		/// <summary>
		/// Gets the value indicating if this IType is mutable.
		/// </summary>
		/// <value>true - a <see cref="MutableType"/> is mutable.</value>
		/// <remarks>
		/// This has been "sealed" because any subclasses are expected to be mutable.  If
		/// the type is immutable then they should inherit from <see cref="ImmutableType"/>.
		/// </remarks>
		public override sealed bool IsMutable 
		{
			get { return true; }
		}
		

		///	<summary>
		///	Gets whether or not this IType contains 
		///	<see cref="System.Type"/>s that implement well-behaived <c>Equals()</c> method.
		///	</summary>
		/// <value>
		/// false - it is assumed that a MutableType does not implement a 
		/// well-behaived <c>Equals()</c>.
		/// </value>
		/// <remarks>
		/// There is no concrete rule that <see cref="MutableType"/>s don't implement
		/// a well-behaived <c>Equals()</c>.  If the <see cref="MutableType"/> does implement
		/// the <c>Equals()</c> then set this to <c>true</c>.
		/// </remarks>
		public override bool HasNiceEquals {
			get { return false; } //default ... may be overridden
		}
	}
}