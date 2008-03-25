using System;
using System.Data;
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
		/// <summary>
		/// Initialize a new instance of the BooleanType
		/// </summary>
		/// <remarks>This is used when the Property is mapped to a native boolean type.</remarks>
		internal BooleanType() : base(SqlTypeFactory.Boolean)
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
		internal BooleanType(AnsiStringFixedLengthSqlType sqlType) : base(sqlType)
		{
		}

		public override object Get(IDataReader rs, int index)
		{
			return Convert.ToBoolean(rs[index]);
		}

		public override object Get(IDataReader rs, string name)
		{
			return Convert.ToBoolean(rs[name]);
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof(bool); }
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(bool); }
		}

		public override void Set(IDbCommand cmd, object value, int index)
		{
			((IDataParameter) cmd.Parameters[index]).Value = (bool) value;
		}

		public override string Name
		{
			get { return "Boolean"; }
		}

		public override object DefaultValue
		{
			get { return false; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return dialect.ToBooleanValueString((bool)value);
		}

		public virtual object StringToObject(string xml)
		{
			return FromStringValue(xml);
		}

		public override object FromStringValue(string xml)
		{
			return bool.Parse(xml);
		}
	}
}