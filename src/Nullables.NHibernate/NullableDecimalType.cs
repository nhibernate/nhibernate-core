using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Nullables.NHibernate
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableDecimal"/>.
	/// </summary>
	public class NullableDecimalType : NullableTypesType
	{
		public NullableDecimalType() : base( new DecimalSqlType() )
		{
		}

		public override object NullValue
		{
			get { return NullableDecimal.Default; }
		}

		public override Type ReturnedClass
		{
			get { return typeof( NullableDecimal ); }
		}

		public override object Get( IDataReader rs, int index )
		{
			return new NullableDecimal( Convert.ToDecimal( rs[ index ] ) );
		}

		public override void Set( IDbCommand cmd, object value, int index )
		{
			IDataParameter parameter = ( IDataParameter ) cmd.Parameters[ index ];
			NullableDecimal nullableValue = ( NullableDecimal ) value;

			if( nullableValue.HasValue )
			{
				parameter.Value = nullableValue.Value;
			}
			else
			{
				parameter.Value = DBNull.Value;
			}
		}

		public override object FromStringValue( string xml )
		{
			return NullableDecimal.Parse( xml );
		}
	}
}
