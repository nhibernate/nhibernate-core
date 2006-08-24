using System;
using System.Data;

using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Nullables.NHibernate
{
	/// <summary>
	/// A NHibernate <see cref="IType"/> for a <see cref="NullableInt16"/>.
	/// </summary>
	[Serializable]
	public class NullableInt16Type : NullableTypesType
	{
		public NullableInt16Type() : base( SqlTypeFactory.Int16 )
		{
		}

		public override object NullValue
		{
			get { return NullableInt16.Default; }
		}

		public override Type ReturnedClass
		{
			get { return typeof( NullableInt16 ); }
		}

		public override object Get( IDataReader rs, int index )
		{
			return new NullableInt16( Convert.ToInt16( rs[ index ] ) );
		}

		public override void Set( IDbCommand cmd, object value, int index )
		{
			IDataParameter parameter = ( IDataParameter ) cmd.Parameters[ index ];
			NullableInt16 nullableValue = ( NullableInt16 ) value;

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
			return NullableInt16.Parse( xml );
		}
	}
}