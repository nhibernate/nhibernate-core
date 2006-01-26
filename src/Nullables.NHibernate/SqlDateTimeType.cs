using System;
using System.Data;
using System.Data.SqlTypes;
using NHibernate.SqlTypes;

namespace Nullables.NHibernate
{
	public class SqlDateTimeType : SqlTypesType
	{
		public SqlDateTimeType() : base( new DateTimeSqlType() )
		{
		}
		
		public override object Get( IDataReader rs, int index )
		{
			return new SqlDateTime( Convert.ToDateTime( rs[ index ] ) );
		}

		protected override object GetValue( INullable value )
		{
			return ( ( SqlDateTime ) value ).Value;
		}

		public override object FromStringValue( string xml )
		{
			return SqlDateTime.Parse( xml );
		}

		public override Type ReturnedClass
		{
			get { return typeof( SqlDateTime ); }
		}
	}
}
