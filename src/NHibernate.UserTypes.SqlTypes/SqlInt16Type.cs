using System;
using System.Data;
using System.Data.SqlTypes;
using NHibernate.SqlTypes;

namespace NHibernate.UserTypes.SqlTypes
{
	public class SqlInt16Type : SqlTypesType
	{
		public SqlInt16Type() : base( new Int16SqlType() )
		{
		}

		public override object Get( IDataReader rs, int index )
		{
			return new SqlInt16( Convert.ToInt16( rs[ index ] ) ); 
		}

		protected override object GetValue( INullable value )
		{
			return ( ( SqlInt16 ) value ).Value;
		}

		public override object FromStringValue( string xml )
		{
			return SqlInt16.Parse( xml );
		}

		public override System.Type ReturnedClass
		{
			get { return typeof( SqlInt16 ); }
		}
	}
}
