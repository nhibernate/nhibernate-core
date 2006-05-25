using System;
using System.Collections;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Int32"/> Property 
	/// to a <see cref="DbType.Int32"/> column.
	/// </summary>
	[Serializable]
	public class Int32Type : ValueTypeType, IDiscriminatorType, IVersionType
	{
		/// <summary></summary>
		internal Int32Type() : base( new Int32SqlType() )
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
			return Convert.ToInt32( rs[ index ] );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, string name )
		{
			return Convert.ToInt32( rs[ name ] );
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( Int32 ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set( IDbCommand cmd, object value, int index )
		{
			IDataParameter parm = cmd.Parameters[ index ] as IDataParameter;
			parm.Value = value;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Int32"; }
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
			return FromString( xml );
		}

		public override object FromStringValue( string xml )
		{
			return int.Parse( xml );
		}

		#region IVersionType Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="current"></param>
		/// <returns></returns>
		public virtual object Next( object current )
		{
			return ( ( int ) current ) + 1;
		}

		/// <summary></summary>
		public virtual object Seed
		{
			get { return 0; }
		}

		public IComparer Comparator
		{
			get { return Comparer.DefaultInvariant; }
		}

		#endregion
	}
}