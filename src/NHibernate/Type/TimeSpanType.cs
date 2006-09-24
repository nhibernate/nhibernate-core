using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.TimeSpan" /> Property to an <see cref="DbType.Int64" /> column 
	/// </summary>
	[Serializable]
	public class TimeSpanType : ValueTypeType, IVersionType, ILiteralType
	{
		/// <summary></summary>
		internal TimeSpanType() : base( SqlTypeFactory.Int64 )
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

		public override string ToString( object val )
		{
			return ( ( TimeSpan ) val ).Ticks.ToString();
		}

		public override bool Equals( object x, object y )
		{
			return object.Equals(x, y);
		}

		public override int GetHashCode(object x, ISessionFactoryImplementor factory)
		{
			return x.GetHashCode();
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public object StringToObject( string xml )
		{
			return TimeSpan.Parse( xml );
		}

		public IComparer Comparator
		{
			get { return Comparer.DefaultInvariant; }
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override object FromStringValue( string xml )
		{
			return TimeSpan.Parse( xml );
		}
	}
}
