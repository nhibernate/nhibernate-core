using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Nullables.NHibernate
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableGuid"/>.
	/// </summary>
	[Serializable]
	public class NullableGuidType : NullableTypesType
	{
		public NullableGuidType() : base( new GuidSqlType() )
		{
		}

		public override object NullValue
		{
			get { return NullableGuid.Default; }
		}

		public override Type ReturnedClass
		{
			get { return typeof( NullableGuid ); }
		}

		public override object Get( IDataReader rs, int index )
		{
			object value = rs[ index ];
			if( value is Guid )
			{
				return new NullableGuid( ( Guid ) value );
			}
			else
			{
				return new NullableGuid( new Guid( value.ToString() ) ); //certain DB's that have no Guid (MySQL) will return strings.
			}
		}

		public override void Set( IDbCommand cmd, object value, int index )
		{
			IDataParameter parameter = ( IDataParameter ) cmd.Parameters[ index ];
			NullableGuid nullableValue = ( NullableGuid ) value;

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
			return new NullableGuid( xml );
		}
	}
}