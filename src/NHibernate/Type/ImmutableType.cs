using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Superclass of nullable immutable types.
	/// </summary>
	[Serializable]
	public abstract class ImmutableType : NullableType
	{
		/// <summary>
		/// Initialize a new instance of the ImmutableType class using a 
		/// <see cref="SqlType"/>. 
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="SqlType"/>.</param>
		protected ImmutableType(SqlType sqlType) : base(sqlType)
		{
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

		public override object Replace(object original, object current, ISessionImplementor session, object owner,
									   IDictionary copiedAlready)
		{
			return original;
		}

		public override object DeepCopy(object value, EntityMode entityMode, ISessionFactoryImplementor factory)
		{
			return value;
		}

	}
}