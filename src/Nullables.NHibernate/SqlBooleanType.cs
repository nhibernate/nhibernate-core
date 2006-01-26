using System;
using System.Data;
using System.Data.SqlTypes;
using NHibernate.SqlTypes;

namespace Nullables.NHibernate
{
	public class SqlBooleanType : SqlTypesType
	{
		public SqlBooleanType() : base( new BooleanSqlType() )
		{
		}

		public override object Get( IDataReader rs, int index )
		{
			return new SqlBoolean( Convert.ToBoolean( rs[ index ] ) );
		}

		protected override object GetValue( INullable value )
		{
			return ( ( SqlBoolean ) value ).Value;
		}

		public override object FromStringValue( string xml )
		{
			return SqlBoolean.Parse( xml );
		}

		public override Type ReturnedClass
		{
			get { return typeof( SqlBoolean ); }
		}
	}
}
