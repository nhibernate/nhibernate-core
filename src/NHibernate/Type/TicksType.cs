using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> Property to an <see cref="DbType.Int64" /> column 
	/// that stores the DateTime using the Ticks property.
	/// </summary>
	/// <remarks>
	/// This is the recommended way to "timestamp" a column.  The System.DateTime.Ticks 
	/// is accurate to 100-nanosecond intervals. 
	/// </remarks>
	public class TicksType : ValueTypeType, IVersionType, ILiteralType
	{
		/// <summary></summary>
		internal TicksType() : base( new Int64SqlType() )
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
			return new DateTime( Convert.ToInt64( rs[ index ] ) );
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
			get { return typeof( DateTime ); }
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
			parm.Value = ( ( DateTime ) value ).Ticks;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Ticks"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public override string ToXML( object val )
		{
			return ( ( DateTime ) val ).Ticks.ToString();
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

			long xTime = ( ( DateTime ) x ).Ticks;
			long yTime = ( ( DateTime ) y ).Ticks;
			return xTime == yTime;
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
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
			get { return DateTime.Now; }
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ObjectToSQLString( object value )
		{
			return "'" + ( ( DateTime ) value ).Ticks.ToString() + "'";
		}
	}
}