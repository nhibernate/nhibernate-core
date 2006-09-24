using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps the Year, Month, and Day of a <see cref="System.DateTime"/> Property to a 
	/// <see cref="DbType.Date"/> column
	/// </summary>
	[Serializable]
	public class DateType : ValueTypeType, IIdentifierType, ILiteralType
	{
		/// <summary></summary>
		internal DateType() : base( SqlTypeFactory.Date )
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
			DateTime dbValue = Convert.ToDateTime( rs[ index ] );
			return new DateTime( dbValue.Year, dbValue.Month, dbValue.Day );
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
			if( ( DateTime ) value < new DateTime( 1753, 1, 1 ) )
			{
				parm.Value = DBNull.Value;
			}
			else
			{
				parm.DbType = DbType.Date;
				parm.Value = value;
			}
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

			DateTime date1 = ( DateTime ) x;
			DateTime date2 = ( DateTime ) y;

			return date1.Day == date2.Day
				&& date1.Month == date2.Month
				&& date1.Year == date2.Year;
		}

		public override int GetHashCode(object x, ISessionFactoryImplementor factory)
		{
			// Custom hash code implementation since DateType is accurate up to days only
			DateTime date = (DateTime)x;
			int hashCode = 1;
			hashCode = 31 * hashCode + date.Day;
			hashCode = 31 * hashCode + date.Month;
			hashCode = 31 * hashCode + date.Year;
			return hashCode;
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "Date"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public override string ToString( object val )
		{
			return ( ( DateTime ) val ).ToShortDateString();
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override object FromStringValue( string xml )
		{
			return DateTime.Parse( xml );
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override string ObjectToSQLString( object value )
		{
			return "'" + ( ( DateTime ) value ).ToShortDateString() + "'";
		}
	}
}
