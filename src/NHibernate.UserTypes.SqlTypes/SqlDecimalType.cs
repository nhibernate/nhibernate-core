using System;
using System.Data;
using System.Data.SqlTypes;

using NHibernate.SqlTypes;

namespace NHibernate.UserTypes.SqlTypes
{
	[Serializable]
	public class SqlDecimalType : SqlTypesType
	{
		public SqlDecimalType() : base(SqlTypeFactory.Decimal)
		{
		}

		public override object Get(IDataReader rs, int index)
		{
			return new SqlDecimal(Convert.ToDecimal(rs[index]));
		}

		protected override object GetValue(INullable value)
		{
			return ((SqlDecimal) value).Value;
		}

		public override object FromStringValue(string xml)
		{
			return SqlDecimal.Parse(xml);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(SqlDecimal); }
		}
	}
}