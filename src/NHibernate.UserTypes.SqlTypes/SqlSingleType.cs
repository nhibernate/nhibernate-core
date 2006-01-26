using System;
using System.Data;
using System.Data.SqlTypes;
using NHibernate.SqlTypes;

namespace NHibernate.UserTypes.SqlTypes
{
	public class SqlSingleType : SqlTypesType
	{
		public SqlSingleType() : base( new SingleSqlType() )
		{
		}

		public override object Get( IDataReader rs, int index )
		{
			return new SqlSingle( Convert.ToSingle( rs[ index ] ) );
		}

		protected override object GetValue( INullable value )
		{
			return ( ( SqlSingle ) value ).Value;
		}

		public override object FromStringValue( string xml )
		{
			return SqlSingle.Parse( xml );
		}

		public override System.Type ReturnedClass
		{
			get { return typeof( SqlSingle ); }
		}
	}
}
