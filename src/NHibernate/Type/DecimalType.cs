using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
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
		public DecimalType()
			: this(SqlTypeFactory.Decimal)
		{
		}

		public DecimalType(SqlType sqlType) : base(sqlType)
		{
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			return Convert.ToDecimal(rs[index]);
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Convert.ToDecimal(rs[name]);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(Decimal); }
		}

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			st.Parameters[index].Value = value;
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