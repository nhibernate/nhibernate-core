using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type 
{
	
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> Property to an DateTime column that only stores the 
	/// Hours, Minutes, and Seconds of the DateTime as significant.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This defaults the Date to "1753-01-01" - that should not matter because
	/// using this Type indicates that you don't care about the Date portion of the DateTime.
	/// </para>
	/// <para>
	/// A more appropriate choice to store the duration/time is the <see cref="TimeSpanType"/>.
	/// The underlying <see cref="DbType.Time"/> tends to be handled diffently by different
	/// DataProviders.
	/// </para>
	/// </remarks>
	public class TimeType : ValueTypeType, IIdentifierType, ILiteralType
	{
		private static DateTime BaseDateValue = new DateTime( 1753, 01, 01 );

		internal TimeType() : base( new TimeSqlType() ) 
		{
		}

		public override object Get(IDataReader rs, int index) 
		{
			DateTime dbValue = Convert.ToDateTime(rs[index]);
			return new DateTime(1753, 01, 01, dbValue.Hour, dbValue.Minute, dbValue.Second);
		}

		public override object Get(IDataReader rs, string name) 
		{
			return Get(rs, rs.GetOrdinal(name));
		}

		public override System.Type ReturnedClass 
		{
			get { return typeof(DateTime); }
		}

		public override void Set(IDbCommand st, object value, int index) 
		{
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			if( (DateTime)value < TimeType.BaseDateValue )
			{
				parm.Value = DBNull.Value;
			}
			else 
			{
				parm.Value = value;
			}
		}

		public override bool Equals(object x, object y) 
		{
			if (x==y) return true;
			if (x==null || y==null) return false;

			DateTime date1 = (DateTime) x;
			DateTime date2 = (DateTime) y;

			return date1.Hour == date2.Hour
				&& date1.Minute== date2.Minute
				&& date1.Second == date2.Second;
		}

		public override string Name 
		{
			get { return "Time"; }
		}

		public override string ToXML(object val) 
		{
			return ((DateTime)val).ToShortTimeString();
		}

		public override bool HasNiceEquals 
		{
			get { return true; }
		}

		public object StringToObject(string xml) 
		{
			return DateTime.Parse(xml);
		}

		public override string ObjectToSQLString(object value) 
		{
			return "'" + ((DateTime)value).ToShortTimeString() + "'";
		}
	}
}

