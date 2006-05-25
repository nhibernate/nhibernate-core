using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.Single" /> Property to an 
	/// <see cref="DbType.Single" /> column.
	/// </summary>
	/// <remarks>
	/// Verify through your database's documentation if there is a column type that
	/// matches up with the capabilities of <see cref="System.Single" />  
	/// </remarks>
	[Serializable]
	public class SingleType : ValueTypeType
	{
		/// <summary></summary>
		internal SingleType() : base( new SingleSqlType() )
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
			return Convert.ToSingle( rs[ index ] );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rs"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object Get( IDataReader rs, string name )
		{
			return Convert.ToSingle( rs[ name ] );
		}

		/// <summary></summary>
		public override System.Type ReturnedClass
		{
			get { return typeof( Single ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="index"></param>
		public override void Set( IDbCommand st, object value, int index )
		{
			IDataParameter parm = st.Parameters[ index ] as IDataParameter;
			parm.Value = value;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Single"; }
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

		public override object FromStringValue( string xml )
		{
			return float.Parse( xml );
		}
	}
}