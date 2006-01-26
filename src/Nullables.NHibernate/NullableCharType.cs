using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Nullables.NHibernate
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableChar"/>.
	/// </summary>
	public class NullableCharType : NullableTypesType
	{
		public NullableCharType() : base( new StringFixedLengthSqlType( 1 ) )
		{
		}

		public override object NullValue
		{
			get { return NullableChar.Default; }
		}

		public override Type ReturnedClass
		{
			get { return typeof( NullableChar ); }
		}

		public override object Get( IDataReader rs, int index )
		{
			return new NullableChar( Convert.ToChar( rs[ index ] ) );
		}

		public override void Set( IDbCommand cmd, object value, int index )
		{
			IDataParameter parameter = ( IDataParameter ) cmd.Parameters[ index ];
			NullableChar nullableValue = ( NullableChar ) value;

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
			return new NullableChar( xml[ 0 ] );
		}
	}
}