using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Char"/> Property 
	/// to a DbType.Char column.
	/// </summary>
	public class CharType : ValueTypeType, IDiscriminatorType
	{
		/// <summary></summary>
		internal CharType() : base( new StringFixedLengthSqlType( 1 ) )
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
			string dbValue = Convert.ToString( rs[ index ] );
			if( dbValue == null )
			{
				return null;
			}
			else
			{
				return dbValue[ 0 ];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, string name )
		{
			return Get( rs, rs.GetOrdinal( name ) );
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( char ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set( IDbCommand cmd, object value, int index )
		{
			( ( IDataParameter ) cmd.Parameters[ index ] ).Value = ( char ) value;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Char"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ObjectToSQLString( object value )
		{
			return '\'' + value.ToString() + '\'';
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public virtual object StringToObject( string xml )
		{
			if( xml.Length != 1 )
			{
				throw new MappingException( "multiple or zero characters found parsing string" );
			}
			return xml[ 0 ];
		}
	}
}