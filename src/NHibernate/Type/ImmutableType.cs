using System;

using NHibernate.SqlTypes;

namespace NHibernate.Type {

	/// <summary>
	/// Superclass of nullable immutable types.
	/// </summary>
	public abstract class ImmutableType : NullableType {

		/// <summary>
		/// Initialize a new instance of the ImmutableType class using a 
		/// <see cref="SqlType"/>. 
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="SqlType"/>.</param>
		protected ImmutableType(SqlType sqlType) : base(sqlType) 
		{
		}

		/// <summary>
		/// Returns a deep copy of the persistent state.
		/// </summary>
		/// <param name="val">The value to deep copy.</param>
		/// <returns>A deep copy of the object.</returns>
		/// <remarks>
		/// This just returns the value passed in because if the object being mapped is 
		/// immutable.
		/// 
		/// TODO: I really need to understand where this method is used so that the 
		/// .NET types inherit from the correct MutableType/ImmutableType IType.
		/// </remarks>
		public override sealed object DeepCopyNotNull(object val) 
		{
			return val;
		}

		/// <summary>
		/// Gets the value indicating if this IType is mutable.
		/// </summary>
		/// <value>false - an <see cref="ImmutableType"/> is not mutable.</value>
		/// <remarks>
		/// This has been "sealed" because any subclasses are expected to be immutable.  If
		/// the type is mutable then they should inherit from <see cref="MutableType"/>.
		/// </remarks>
		public override sealed bool IsMutable 
		{
			get { return false; }
		}

		///	<summary>
		///	Gets whether or not this IType contains 
		///	<see cref="System.Type"/>s that implement well-behaived <c>Equals()</c> method.
		///	</summary>
		/// <value>
		/// true - it is assumed that a ImmutableType implements a 
		/// well-behaived <c>Equals()</c>.
		/// </value>
		/// <remarks>
		/// There is no concrete rule that <see cref="ImmutableType"/>s implement
		/// a well-behaived <c>Equals()</c>.  If the <see cref="ImmutableType"/> does 
		/// not implement the <c>Equals()</c> then set this to <c>false</c>.
		/// </remarks>
		public override bool HasNiceEquals 
		{
			get { return true; }
		}
		
	}
}