using System;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
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

		/// <summary>
		/// Default constructor.
		/// </summary>
		public TimeType() : base(SqlTypeFactory.Time)
		{
		}

		/// <summary>
		/// Constructor for specifying a time with a scale. Use <see cref="SqlTypeFactory.GetTime"/>.
		/// </summary>
		/// <param name="sqlType">The sql type to use for the type.</param>
		public TimeType(TimeSqlType sqlType) : base(sqlType)
		{
		}

		public override string Name
		{
			get { return "Time"; }
		}

		public override object Get(DbDataReader rs, int index, ISessionImplementor session)
		{
			try
			{
				if (rs[index] is TimeSpan time) //For those dialects where DbType.Time means TimeSpan.
				{
					return BaseDateValue.AddTicks(time.Ticks);
				}

				DateTime dbValue = Convert.ToDateTime(rs[index]);
				return BaseDateValue.Add(dbValue.TimeOfDay);
			}
			catch (Exception ex)
			{
				throw new FormatException(string.Format("Input string '{0}' was not in the correct format.", rs[index]), ex);
			}
		}

		public override object Get(DbDataReader rs, string name, ISessionImplementor session)
		{
			return Get(rs, rs.GetOrdinal(name), session);
		}

		public override System.Type ReturnedClass
		{
			get { return typeof(DateTime); }
		}

		public override void Set(DbCommand st, object value, int index, ISessionImplementor session)
		{
			var dateTime = (DateTime)value;
			if (dateTime < BaseDateValue)
			{
				st.Parameters[index].Value = DBNull.Value;
				return;
			}
			if (session.Factory.ConnectionProvider.Driver.RequiresTimeSpanForTime)
				st.Parameters[index].Value = dateTime.TimeOfDay;
			else
				st.Parameters[index].Value = dateTime;
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
			if (date1.Equals(date2))
				return true;

			return date1.Hour == date2.Hour
				&& date1.Minute == date2.Minute
				&& date1.Second == date2.Second;
		}

		public override int GetHashCode(object x)
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

		/// <inheritdoc />
		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			return (value == null) ? null :
				// 6.0 TODO: inline this call.
#pragma warning disable 618
				ToString(value);
#pragma warning restore 618
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version. Override ToLoggableString instead.")]
		public override string ToString(object val)
		{
			return ((DateTime)val).ToString("T");
		}

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc />
		public object StringToObject(string xml)
		{
			return string.IsNullOrEmpty(xml) ? null :
				// 6.0 TODO: inline the call.
#pragma warning disable 618
				FromStringValue(xml);
#pragma warning restore 618
		}

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
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
