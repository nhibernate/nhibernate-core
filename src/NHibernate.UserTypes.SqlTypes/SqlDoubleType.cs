using System;
using System.Data;
using System.Data.SqlTypes;
using NHibernate.SqlTypes;

namespace NHibernate.UserTypes.SqlTypes
{
	public class SqlDoubleType : SqlTypesType
	{
		public SqlDoubleType() : base( new DoubleSqlType() )
		{
		}

		public override object Get( IDataReader rs, int index )
		{
			return new SqlDouble( Convert.ToDouble( rs[ index ] ) );
		}

		protected override object GetValue( INullable value )
		{
			return ( ( SqlDouble ) value ).Value;
		}

		public override object FromStringValue( string xml )
		{
			return SqlDouble.Parse( xml );
		}

		public override System.Type ReturnedClass
		{
			get { return typeof( SqlDouble ); }
		}
	}
}
