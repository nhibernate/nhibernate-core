using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Decimal"/> Property 
	/// to a <see cref="DbType.Decimal"/> column.
	/// </summary>
	[Serializable]
	public class DecimalType : PrimitiveType, IIdentifierType
	{
		internal DecimalType()
			: this(SqlTypeFactory.Decimal)
		{
		}

		internal DecimalType(SqlType sqlType) : base(sqlType)
		{
		}

		public override object Get(IDataReader rs, int index)
		{
			return Convert.ToDecimal(rs[index]);
		}

		public override object Get(IDataReader rs, string name)
		{
			return Convert.ToDecimal(rs[name]);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(Decimal); }
		}

		public override void Set(IDbCommand st, object value, int index)
		{
			((IDataParameter) st.Parameters[index]).Value = value;
		}

		public override string Name
		{
			get { return "Decimal"; }
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof (Decimal); }
		}

		public override object DefaultValue
		{
			get { return 0m; }
		}

		public override object FromStringValue(string xml)
		{
			return Decimal.Parse(xml);
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return value.ToString();
		}

		public object StringToObject(string xml)
		{
			return FromStringValue(xml);
		}
	}
}