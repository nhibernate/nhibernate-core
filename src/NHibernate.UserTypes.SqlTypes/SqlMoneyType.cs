using System;
using System.Data;
using System.Data.SqlTypes;

using NHibernate.SqlTypes;

namespace NHibernate.UserTypes.SqlTypes
{
	[Serializable]
	public class SqlMoneyType : SqlTypesType
	{
		public SqlMoneyType() : base(SqlTypeFactory.Currency)
		{
		}

		public override object Get(IDataReader rs, int index)
		{
			return new SqlMoney(Convert.ToDecimal(rs[index]));
		}

		protected override object GetValue(INullable value)
		{
			return ((SqlMoney) value).Value;
		}

		public override object FromStringValue(string xml)
		{
			return SqlMoney.Parse(xml);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(SqlMoney); }
		}
	}
}