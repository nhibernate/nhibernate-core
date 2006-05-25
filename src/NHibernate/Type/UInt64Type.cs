using System;
using System.Collections;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.UInt64"/> Property 
	/// to a <see cref="DbType.UInt64"/> column.
	/// </summary>
	[Serializable]
	public class UInt64Type : ValueTypeType, IDiscriminatorType, IVersionType
	{
		/// <summary></summary>
		public UInt64Type() : base( new UInt64SqlType() )
		{
		}

		public override object Get( IDataReader rs, int index )
		{
			return Convert.ToUInt64( rs[ index ] );
		}

		public override object Get( IDataReader rs, string name )
		{
			return Convert.ToUInt64( rs[ name ] );
		}

		public override System.Type ReturnedClass
		{
			get { return typeof( UInt64 ); }
		}

		public override void Set( IDbCommand rs, object value, int index )
		{
			IDataParameter parm = rs.Parameters[ index ] as IDataParameter;
			parm.Value = value;
		}

		public override string Name
		{
			get { return "UInt64"; }
		}

		public override string ObjectToSQLString( object value )
		{
			return value.ToString();
		}

		public virtual object StringToObject( string xml )
		{
			return FromStringValue( xml );
		}

		public override object FromStringValue( string xml )
		{
			return ulong.Parse( xml );
		}

		#region IVersionType Members

		public virtual object Next( object current )
		{
			return ( ulong ) current + 1;
		}

		public virtual object Seed
		{
			get { return ( ulong ) 1; }
		}

		public virtual IComparer Comparator
		{
			get { return Comparer.DefaultInvariant; }
		}

		#endregion
	}
}