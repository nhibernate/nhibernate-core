using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.TimeSpan" /> Property to an <see cref="DbType.Int64" /> column 
	/// </summary>
	public class TimeSpanType : ValueTypeType, IVersionType, ILiteralType
	{
		/// <summary></summary>
		internal TimeSpanType() : base( new Int64SqlType() )
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
			return new TimeSpan( Convert.ToInt64( rs[ index ] ) );
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
			get { return typeof( TimeSpan ); }
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
			parm.Value = ( ( TimeSpan ) value ).Ticks;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "TimeSpan"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public override string ToXML( object val )
		{
			return ( ( TimeSpan ) val ).Ticks.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override bool Equals( object x, object y )
		{
			if( x == y )
			{
				return true;
			}
			if( x == null || y == null )
			{
				return false;
			}

			long xTime = ( ( TimeSpan ) x ).Ticks;
			long yTime = ( ( TimeSpan ) y ).Ticks;
			return xTime == yTime;
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return true; }
		}

		#region IVersionType Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="current"></param>
		/// <returns></returns>
		public object Next( object current )
		{
			return Seed;
		}

		/// <summary></summary>
		public object Seed
		{
			get { return new TimeSpan( DateTime.Now.Ticks ); }
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ObjectToSQLString( object value )
		{
			return "'" + ( ( TimeSpan ) value ).Ticks.ToString() + "'";
		}
	}
}