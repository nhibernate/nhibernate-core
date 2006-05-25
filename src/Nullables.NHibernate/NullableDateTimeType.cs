using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Nullables.NHibernate
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableDateTime"/>.
	/// </summary>
	[Serializable]
	public class NullableDateTimeType : NullableTypesType
	{
		public NullableDateTimeType() : base( new DateTimeSqlType() )
		{
		}

		public override object NullValue
		{
			get { return NullableDateTime.Default; }
		}

		public override Type ReturnedClass
		{
			get { return typeof( NullableDateTime ); }
		}

		public override object Get( IDataReader rs, int index )
		{
			return new NullableDateTime( Convert.ToDateTime( rs[ index ] ) );
		}

		public override void Set( IDbCommand cmd, object value, int index )
		{
			IDataParameter parameter = ( IDataParameter ) cmd.Parameters[ index ];
			NullableDateTime nullableValue = ( NullableDateTime ) value;

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
			return NullableDateTime.Parse( xml );
		}
	}
}
