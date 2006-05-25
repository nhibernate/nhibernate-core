using System;
using System.Collections;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.UInt16"/> Property 
	/// to a <see cref="DbType.UInt16"/> column.
	/// </summary>
	[Serializable]
	public class UInt16Type : ValueTypeType, IDiscriminatorType, IVersionType
	{
		/// <summary></summary>
		internal UInt16Type() : base( new UInt16SqlType() )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, int index )
		{
			return Convert.ToUInt16( rs[ index ] );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, string name )
		{
			return Convert.ToUInt16( rs[ name ] );
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( UInt16 ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set( IDbCommand rs, object value, int index )
		{
			IDataParameter parm = rs.Parameters[ index ] as IDataParameter;
			parm.Value = value;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "UInt16"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ObjectToSQLString( object value )
		{
			return value.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public object StringToObject( string xml )
		{
			return FromStringValue( xml );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override object FromStringValue( string xml )
		{
			return ushort.Parse( xml );
		}

		#region IVersionType Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="current"></param>
		/// <returns></returns>
		public object Next( object current )
		{
			return ( ushort ) ( ( ushort ) current + 1 );
		}

		/// <summary></summary>
		public object Seed
		{
			get { return ( ushort ) 1; }
		}

		public IComparer Comparator
		{
			get { return Comparer.DefaultInvariant; }
		}

		#endregion
	}
}