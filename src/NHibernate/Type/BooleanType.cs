using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Boolean"/> Property 
	/// to a <see cref="DbType.Boolean"/> column.
	/// </summary>
	public class BooleanType : ValueTypeType, IDiscriminatorType
	{
		private static readonly string TRUE = "1";
		private static readonly string FALSE = "0";

		/// <summary>
		/// Initialize a new instance of the BooleanType
		/// </summary>
		/// <remarks>This is used when the Property is mapped to a native boolean type.</remarks>
		internal BooleanType() : base( new BooleanSqlType() )
		{
		}

		/// <summary>
		/// Initialize a new instance of the BooleanType class using a
		/// <see cref="AnsiStringFixedLengthSqlType"/>.
		/// </summary>
		/// <param name="sqlType">The underlying <see cref="SqlType"/>.</param>
		/// <remarks>
		/// This is used when the Property is mapped to a string column
		/// that stores true or false as a string.
		/// </remarks>
		internal BooleanType( AnsiStringFixedLengthSqlType sqlType ) : base( sqlType )
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
			return Convert.ToBoolean( rs[ index ] );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, string name )
		{
			return Convert.ToBoolean( rs[ name ] );
		}

		/// <summary>
		/// 
		/// </summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( bool ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set( IDbCommand cmd, object value, int index )
		{
			( ( IDataParameter ) cmd.Parameters[ index ] ).Value = ( bool ) value;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Boolean"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ObjectToSQLString( object value )
		{
			return ( ( bool ) value ) ? TRUE : FALSE;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public virtual object StringToObject( string xml )
		{
			return bool.Parse( xml );
		}
	}
}