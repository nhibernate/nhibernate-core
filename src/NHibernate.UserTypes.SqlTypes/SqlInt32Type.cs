using System;
using System.Data;
using System.Data.SqlTypes;
using NHibernate.SqlTypes;

namespace NHibernate.UserTypes.SqlTypes
{
	[Serializable]
	public class SqlInt32Type : SqlTypesType
	{
		public SqlInt32Type() : base( SqlTypeFactory.Int32 )
		{
		}

		public override System.Type ReturnedClass
		{
			get { return typeof( SqlInt32 ); }
		}

		public override object Get( IDataReader rs, int index )
		{
			return new SqlInt32( Convert.ToInt32( rs[ index ] ) );
		}

		public override object FromStringValue( string xml )
		{
			return SqlInt32.Parse( xml );
		}

		protected override object GetValue( INullable value )
		{
			return ( ( SqlInt32 ) value ).Value;
		}
	}
}