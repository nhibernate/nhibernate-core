using System;
using System.Collections;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using System.Collections.Generic;

namespace NHibernate.Type
{
	/// <summary>
	/// Maps a <see cref="System.DateTime" /> Property to a <see cref="DbType.DateTime"/> column that 
	/// stores date &amp; time down to the accuracy of a second.
	/// </summary>
	/// <remarks>
	/// This only stores down to a second, so if you are looking for the most accurate
	/// date and time storage your provider can give you use the <see cref="TimestampType" />. 
	/// or the <see cref="TicksType"/>
	/// </remarks>
	[Serializable]
	public class DateTimeType : PrimitiveType, IIdentifierType, ILiteralType, IVersionType
	{
		private static readonly DateTime BaseDateValue = DateTime.MinValue;

		/// <summary></summary>
		public DateTimeType() : base(SqlTypeFactory.DateTime)
		{
		}

		public DateTimeType(SqlType sqlTypeDateTime) : base(sqlTypeDateTime)
		{
		}

		/// <summary></summary>
		public override string Name
		{
			get { return "DateTime"; }
		}

		public override object Get(IDataReader rs, int index)
		{
			try
			{
				DateTime dbValue = Convert.ToDateTime(rs[index]);
				return new DateTime(dbValue.Year, dbValue.Month, dbValue.Day, dbValue.Hour, dbValue.Minute, dbValue.Second);
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
			DateTime dateValue = (DateTime) value;
			((IDataParameter)st.Parameters[index]).Value =
				new DateTime(dateValue.Year, dateValue.Month, dateValue.Day, dateValue.Hour, dateValue.Minute, dateValue.Second);
		}

		#region IVersionType Members

		public virtual object Next(object current, ISessionImplementor session)
		{
			return Seed(session);
		}

		public virtual object Seed(ISessionImplementor session)
		{
			return TimestampType.Round(DateTime.Now, TimeSpan.TicksPerSecond);
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

			DateTime date1 = (DateTime) x;
			DateTime date2 = (DateTime) y;

			if(date1.Equals(date2))
				return true;

			return (date1.Year == date2.Year &&
					date1.Month == date2.Month &&
					date1.Day == date2.Day &&
					date1.Hour == date2.Hour &&
					date1.Minute == date2.Minute &&
					date1.Second == date2.Second);
		}

		public virtual IComparer Comparator
		{
			get { return Comparer<DateTime>.Default; }
		}

		#endregion

		public override int GetHashCode(object x, EntityMode entityMode)
		{
			// Custom hash code implementation because DateTimeType is only accurate
			// up to seconds.
			DateTime date = (DateTime) x;
			int hashCode = 1;
			unchecked
			{
				hashCode = 31 * hashCode + date.Second;
				hashCode = 31 * hashCode + date.Minute;
				hashCode = 31 * hashCode + date.Hour;
				hashCode = 31 * hashCode + date.Day;
				hashCode = 31 * hashCode + date.Month;
				hashCode = 31 * hashCode + date.Year;
			}
			return hashCode;
		}

		public override string ToString(object val)
		{
			return ((DateTime) val).ToString();
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
			return "'" + ((DateTime)value) + "'";
		}
	}
}