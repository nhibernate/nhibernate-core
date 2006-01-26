using System;
using System.Data;
using System.Data.SqlTypes;
using NHibernate.SqlTypes;

namespace Nullables.NHibernate
{
	public class SqlStringType : SqlTypesType
	{
		public SqlStringType() : base( new StringSqlType() )
		{			
		}

		public override object Get( IDataReader rs, int index )
		{
			return new SqlString( Convert.ToString( rs[ index ] ) );
		}

		protected override object GetValue( INullable value )
		{
			return ( ( SqlString ) value ).Value;
		}

		public override object FromStringValue( string xml )
		{
			return new SqlString( xml );
		}

		public override Type ReturnedClass
		{
			get { return typeof( SqlString ); }
		}
	}
}
