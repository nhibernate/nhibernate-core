using System;
using System.Data;
using System.Data.SqlTypes;
using NHibernate.SqlTypes;

namespace NHibernate.UserTypes.SqlTypes
{
	public class SqlGuidType : SqlTypesType
	{
		public SqlGuidType() : base( new GuidSqlType() )
		{
		}

		public override object Get( IDataReader rs, int index )
		{
			return new SqlGuid( rs.GetGuid( index ) );
		}

		protected override object GetValue( INullable value )
		{
			return ( ( SqlGuid ) value ).Value;
		}

		public override object FromStringValue( string xml )
		{
			return SqlGuid.Parse( xml );
		}

		public override System.Type ReturnedClass
		{
			get { return typeof( SqlGuid ); }
		}
	}
}
