using System;
using System.Data;
using System.Data.SqlTypes;
using NHibernate.SqlTypes;

namespace NHibernate.UserTypes.SqlTypes
{
	public class SqlInt64Type : SqlTypesType
	{
		public SqlInt64Type() : base( new Int64SqlType() )
		{
		}

		public override object Get( IDataReader rs, int index )
		{
			return new SqlInt64( Convert.ToInt64( rs[ index ] ) );
		}

		protected override object GetValue( INullable value )
		{
			return ( ( SqlInt64 ) value ).Value;
		}

		public override object FromStringValue( string xml )
		{
			return SqlInt64.Parse( xml );
		}

		public override System.Type ReturnedClass
		{
			get { return typeof( SqlInt64 ); }
		}
	}
}
