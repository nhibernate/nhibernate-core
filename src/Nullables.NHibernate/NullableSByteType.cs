using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Nullables.NHibernate
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableSByte"/>.
	/// </summary>
	[Serializable]
	public class NullableSByteType : NullableTypesType
	{
		public NullableSByteType() : base( SqlTypeFactory.SByte )
		{
		}

		public override object NullValue
		{
			get { return NullableSByte.Default; }
		}

		public override Type ReturnedClass
		{
			get { return typeof( NullableSByte ); }
		}

		public override object Get( IDataReader rs, int index )
		{
			return new NullableSByte( Convert.ToSByte( rs[ index ] ) );
		}

		public override void Set( IDbCommand cmd, object value, int index )
		{
			IDataParameter parameter = ( IDataParameter ) cmd.Parameters[ index ];
			NullableSByte nullableValue = ( NullableSByte ) value;

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
			return NullableSByte.Parse( xml );
		}
	}
}