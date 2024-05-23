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
	public class BooleanType(AnsiStringFixedLengthSqlType sqlType) : PrimitiveType(sqlType), IDiscriminatorType
	{
		protected static readonly object TrueObject = true;
		protected static readonly object FalseObject = false;

		public BooleanType() : this((AnsiStringFixedLengthSqlType) SqlTypeFactory.Boolean)
		{
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return GetBooleanAsObject(Convert.ToBoolean(rs[index]));
		}

		public override System.Type PrimitiveClass => typeof(bool);

		public override System.Type ReturnedClass => typeof(bool);

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
