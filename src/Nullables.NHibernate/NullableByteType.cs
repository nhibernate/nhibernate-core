using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Nullables.NHibernate
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableByte"/>.
	/// </summary>
	[Serializable]
	public class NullableByteType : NullableTypesType
	{
		public NullableByteType() : base( SqlTypeFactory.Byte )
		{
		}

		public override object NullValue
		{
			get { return NullableByte.Default; }
		}

		public override Type ReturnedClass
		{
			get { return typeof( NullableByte ); }
		}

		public override object Get( IDataReader rs, int index )
		{
			return new NullableByte( Convert.ToByte( rs[ index ] ) );
		}

		public override void Set( IDbCommand cmd, object value, int index )
		{
			IDataParameter parameter = ( IDataParameter ) cmd.Parameters[ index ];
			NullableByte nullableValue = ( NullableByte ) value;

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
			return NullableByte.Parse( xml );
		}
	}
}
