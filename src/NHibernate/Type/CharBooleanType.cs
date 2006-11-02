using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Boolean"/> Property 
	/// to a <see cref="DbType.AnsiStringFixedLength"/> column.
	/// </summary>
	[Serializable]
	public abstract class CharBooleanType : BooleanType
	{
		/// <summary></summary>
		protected abstract string TrueString { get; }
		
		/// <summary></summary>
		protected abstract string FalseString { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlType"></param>
		internal CharBooleanType( AnsiStringFixedLengthSqlType sqlType ) : base( sqlType )
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
			string code = Convert.ToString( rs[ index ] );
			if( code == null )
			{
				return null;
			}
			else
			{
				return code.ToUpper( System.Globalization.CultureInfo.InvariantCulture ).Equals( TrueString );
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set( IDbCommand cmd, Object value, int index )
		{
			( ( IDataParameter ) cmd.Parameters[ index ] ).Value = ( ( ( bool ) value ) ? TrueString : FalseString );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ObjectToSQLString( object value )
		{
			return "'" + ( ( ( bool ) value ) ? TrueString : FalseString ) + "'";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override object StringToObject( String xml )
		{
			if( TrueString.Equals( xml.ToUpper( System.Globalization.CultureInfo.InvariantCulture ) ) )
			{
				return true;
			}
			else if( FalseString.Equals( xml.ToUpper( System.Globalization.CultureInfo.InvariantCulture ) ) )
			{
				return false;
			}
			else
			{
				throw new HibernateException( "Could not interpret: " + xml );
			}
		}
	}
}