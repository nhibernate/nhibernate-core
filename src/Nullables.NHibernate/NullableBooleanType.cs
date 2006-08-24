using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Nullables.NHibernate
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableBoolean"/>.
	/// </summary>
	[Serializable]
	public class NullableBooleanType : NullableTypesType
	{
		public NullableBooleanType() : base( SqlTypeFactory.Boolean )
		{
		}

		public override object NullValue
		{
			get { return NullableBoolean.Default; }
		}

		public override Type ReturnedClass
		{
			get { return typeof( NullableBoolean ); }
		}

		public override object Get( IDataReader rs, int index )
		{
			return new NullableBoolean( Convert.ToBoolean( rs[ index ] ) );
		}

		public override void Set( IDbCommand cmd, object value, int index )
		{
			IDataParameter parameter = ( IDataParameter ) cmd.Parameters[ index ];
			NullableBoolean nullableValue = ( NullableBoolean ) value;

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
			return NullableBoolean.Parse( xml );
		}
	}
}