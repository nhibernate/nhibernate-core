using System;
using System.Data;

using NHibernate;
using NHibernate.Type;
using NHibernate.SqlTypes;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Abstract type used for implementing NHibernate <see cref="IType"/>s for 
	/// the Nullables library.
	/// </summary>
	public abstract class NullableTypesType : ImmutableType
	{
		public NullableTypesType( SqlType type ) : base( type )
		{
		}

		public override object NullSafeGet( IDataReader rs, string name )
		{
			object value = base.NullSafeGet( rs, name );
			if( value == null )
			{
				return NullValue;
			}
			else
			{
				return value;
			}
		}

		public override object Get( IDataReader rs, string name )
		{
			return Get( rs, rs.GetOrdinal( name ) );
		}

		public override string ToString( object value )
		{
			return value.ToString();
		}

		public override string Name
		{
			get { return ReturnedClass.Name; }
		}

		public override bool Equals( object x, object y )
		{
			return object.Equals( x, y );
		}

		public abstract object NullValue { get; }
	}
}
