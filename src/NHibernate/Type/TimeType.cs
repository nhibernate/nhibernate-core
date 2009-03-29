using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> Property to an DateTime column that only stores the 
	/// Hours, Minutes, and Seconds of the DateTime as significant.
	/// Also you have for <see cref="DbType.Time"/> handling, the NHibernate Type <see cref="TimeAsTimeSpanType"/>,
	/// the which maps to a <see cref="TimeSpan"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This defaults the Date to "1753-01-01" - that should not matter because
	/// using this Type indicates that you don't care about the Date portion of the DateTime.
	/// </para>
	/// <para>
	/// A more appropriate choice to store the duration/time is the <see cref="TimeSpanType"/>.
	/// The underlying <see cref="DbType.Time"/> tends to be handled differently by different
	/// DataProviders.
	/// </para>
	/// </remarks>
	[Serializable]
	public class TimeType : PrimitiveType, IIdentifierType, ILiteralType
	{
		private static readonly DateTime BaseDateValue = new DateTime(1753, 01, 01);

		internal TimeType() : base(SqlTypeFactory.Time)
		{
		}

		public override string Name
		{
			get { return "Time"; }
		}

		public override object Get(IDataReader rs, int index)
		{
			try
			{
				if(rs[index] is TimeSpan) //For those dialects where DbType.Time means TimeSpan.
				{
					TimeSpan time = (TimeSpan) rs[index];
					return BaseDateValue.AddTicks(time.Ticks);
				}

				DateTime dbValue = Convert.ToDateTime(rs[index]);
				return new DateTime(1753, 01, 01, dbValue.Hour, dbValue.Minute, dbValue.Second);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
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
			((IDataParameter)st.Parameters[index]).Value = ((DateTime)value >= BaseDateValue) ? value : DBNull.Value;
		}

		public override bool IsEqual(object x, object y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}

			DateTime date1 = (DateTime)x;
			DateTime date2 = (DateTime)y;
			if(date1.Equals(date2)) 
				return true;

			return date1.Hour == date2.Hour
						 && date1.Minute == date2.Minute
						 && date1.Second == date2.Second;
		}

		public override int GetHashCode(object x, EntityMode entityMode)
		{
			DateTime date = (DateTime)x;
			int hashCode = 1;
			unchecked
			{
				hashCode = 31 * hashCode + date.Second;
				hashCode = 31 * hashCode + date.Minute;
				hashCode = 31 * hashCode + date.Hour;
			}
			return hashCode;
		}

		public override string ToString(object val)
		{
			return ((DateTime) val).ToShortTimeString();
		}

		public object StringToObject(string xml)
		{
			return string.IsNullOrEmpty(xml) ? null : FromStringValue(xml);
		}

		public override object FromStringValue(string xml)
		{
			return DateTime.Parse(xml);
		}

		public override System.Type PrimitiveClass
		{
			get { return typeof(DateTime); }
		}

		public override object DefaultValue
		{
			get { return BaseDateValue; }
		}

		public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
		{
			return "'" + ((DateTime)value).ToShortTimeString() + "'";
		}
	}
}
