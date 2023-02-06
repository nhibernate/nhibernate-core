using System;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Boolean"/> Property 
	/// to a <see cref="DbType.Boolean"/> column.
	/// </summary>
	[Serializable]
	public class BooleanType : PrimitiveType, IDiscriminatorType
	{
		protected static readonly object TrueObject = true;
		protected static readonly object FalseObject = false;

		/// <summary>
		/// Initialize a new instance of the BooleanType
		/// </summary>
		/// <remarks>This is used when the Property is mapped to a native boolean type.</remarks>
		public BooleanType() : base(SqlTypeFactory.Boolean)
		{
		}

		/// <summary>
		/// Initialize a new instance of the BooleanType class using a
		/// <see cref="AnsiStringFixedLengthSqlType"/>.
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="SqlType"/>.</param>
		/// <remarks>
		/// This is used when the Property is mapped to a string column
		/// that stores true or false as a string.
		/// </remarks>
		public BooleanType(AnsiStringFixedLengthSqlType sqlType) : base(sqlType)
		{
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return GetBooleanAsObject(Convert.ToBoolean(rs[index]));
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return GetBooleanAsObject(Convert.ToBoolean(rs[name]));
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof(bool); }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(bool); }
		}

		public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			cmd.Parameters[index].Value = (bool) value;
		}

		public override string Name => "Boolean";

		public override object DefaultValue => FalseObject;

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return dialect.ToBooleanValueString((bool)value);
		}

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc />
		public virtual object StringToObject(string xml)
		{
			// 6.0 TODO: inline the call
#pragma warning disable 618
			return FromStringValue(xml);
#pragma warning restore 618
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
		public override object FromStringValue(string xml)
		{
			return GetBooleanAsObject(bool.Parse(xml));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static object GetBooleanAsObject(bool value)
		{
			return value ? TrueObject : FalseObject;
		}
	}
}
