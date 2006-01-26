using System;
using System.Data;
using System.Data.SqlTypes;
using NHibernate.SqlTypes;

namespace Nullables.NHibernate
{
	public class SqlMoneyType : SqlTypesType
	{
		public SqlMoneyType() : base( new CurrencySqlType() )
		{
		}

		public override object Get( IDataReader rs, int index )
		{
			return new SqlMoney( Convert.ToDecimal( rs[ index ] ) );
		}

		protected override object GetValue( INullable value )
		{
			return ( ( SqlMoney ) value ).Value;
		}

		public override object FromStringValue( string xml )
		{
			return SqlMoney.Parse( xml );
		}

		public override Type ReturnedClass
		{
			get { return typeof( SqlMoney ); }
		}
	}
}
