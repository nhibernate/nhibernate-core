using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.SByte"/> Property 
	/// to a <see cref="DbType.SByte"/> column.
	/// </summary>
	public class SByteType : ValueTypeType, IDiscriminatorType
	{
		/// <summary></summary>
		internal SByteType() : base( new SByteSqlType() )
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
			return Convert.ToSByte( rs[ index ] );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, string name )
		{
			return Convert.ToSByte( rs[ name ] );
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( SByte ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set( IDbCommand cmd, object value, int index )
		{
			( ( IDataParameter ) cmd.Parameters[ index ] ).Value = ( SByte ) value;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "SByte"; }
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
		public virtual object StringToObject( string xml )
		{
			return SByte.Parse( xml );
		}
	}
}