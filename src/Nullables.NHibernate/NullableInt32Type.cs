using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Nullables.NHibernate
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableInt32"/>.
	/// </summary>
	[Serializable]
	public class NullableInt32Type : NullableTypesType
	{
		public NullableInt32Type() : base( new Int32SqlType() )
		{
		}

		public override object NullValue
		{
			get { return NullableInt32.Default; }
		}

		public override Type ReturnedClass
		{
			get { return typeof( NullableInt32 ); }
		}

		public override object Get( IDataReader rs, int index )
		{
			return new NullableInt32( Convert.ToInt32( rs[ index ] ) );
		}

		public override void Set( IDbCommand cmd, object value, int index )
		{
			IDataParameter parameter = ( IDataParameter ) cmd.Parameters[ index ];
			NullableInt32 nullableValue = ( NullableInt32 ) value;

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
			return NullableInt32.Parse( xml );
		}
	}
}