using System;
using NHibernate.Engine;
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

		/// <inheritdoc />
		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return (value == null) ? null :
				// 6.0 TODO: inline this call.
#pragma warning disable 618
				ToString(value);
#pragma warning restore 618
		}

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
		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version. Override ToLoggableString instead.")]
		public override string ToString(object val)
		{
			return val.ToString();
		}
	}
}
