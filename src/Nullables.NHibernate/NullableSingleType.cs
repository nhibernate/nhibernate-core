using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Nullables.NHibernate
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableSingle"/>.
	/// </summary>
	public class NullableSingleType : NullableTypesType
	{
		public NullableSingleType() : base( new SingleSqlType() )
		{
		}

		public override object NullValue
		{
			get { return NullableSingle.Default; }
		}

		public override Type ReturnedClass
		{
			get { return typeof( NullableSingle ); }
		}

		public override object Get( IDataReader rs, int index )
		{
			return new NullableSingle( Convert.ToSingle( rs[ index ] ) );
		}

		public override void Set( IDbCommand cmd, object value, int index )
		{
			IDataParameter parameter = ( IDataParameter ) cmd.Parameters[ index ];
			NullableSingle nullableValue = ( NullableSingle ) value;

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
			return NullableSingle.Parse( xml );
		}
	}
}
