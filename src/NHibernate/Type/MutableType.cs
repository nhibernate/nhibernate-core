using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Superclass for mutable nullable types.
	/// </summary>
	[Serializable]
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
		/// well-behaved <c>Equals()</c>.
		/// </value>
		/// <remarks>
		/// There is no concrete rule that <see cref="MutableType"/>s don't implement
		/// a well-behaved <c>Equals()</c>.  If the <see cref="MutableType"/> does implement
		/// the <c>Equals()</c> then set this to <see langword="true" />.
		/// </remarks>
		public override bool HasNiceEquals
		{
			get { return false; } //default ... may be overridden
		}

		public override object Replace(object original, object current, ISessionImplementor session, object owner,
		                               IDictionary copiedAlready)
		{
			return DeepCopy(original);
		}
	}
}