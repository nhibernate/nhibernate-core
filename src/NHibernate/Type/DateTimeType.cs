using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type 
{
	
	/// <summary>
	/// Maps a System.DateTime Property to a column that stores date & time down to 
	/// the accuracy of a second.
	/// </summary>
	/// <remarks>
	/// This only stores down to a second, so if you are looking for the most accurate
	/// date and time storage your provider can give you use the <see cref="TimestampType" />. 
	/// or the <see cref="TicksType"/>
	/// </remarks>
	public class DateTimeType : MutableType, IIdentifierType, ILiteralType, IVersionType
	{
		
		public DateTimeType(DateTimeSqlType sqlType) : base(sqlType) 
		{
		}

		public override object Get(IDataReader rs, int index) 
		{
			DateTime dbValue = rs.GetDateTime(index);
			return new DateTime(dbValue.Year, dbValue.Month, dbValue.Day, dbValue.Hour, dbValue.Minute, dbValue.Second);
		}

		public override object Get(IDataReader rs, string name) 
		{
			return Get(rs,rs.GetOrdinal(name));// rs.[name];
		}

		public override System.Type ReturnedClass 
		{
			get { return typeof(DateTime); }
		}

		public override void Set(IDbCommand st, object value, int index) 
		{
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.DbType = DbType.DateTime;
			//TODO: figure out if this is a good solution for NULL DATES
			if(value.Equals(System.DateTime.MinValue)) 
			{
				parm.Value = DBNull.Value;
			}
			else 
			{
				DateTime dateValue = (DateTime)value;
				parm.Value = new DateTime(dateValue.Year, dateValue.Month, dateValue.Day, dateValue.Hour, dateValue.Minute, dateValue.Second);
			}
		}

		public override bool Equals(object x, object y) 
		{
			if (x==y) return true;
			// DateTime can't be null because it is a struct - so comparing 
			// them this way is useless - instead use the magic number...
			//if (x==null || y==null) return false;

			DateTime date1 = (x==null)? DateTime.MinValue : (DateTime) x;
			DateTime date2 = (y==null)? DateTime.MinValue : (DateTime) y;

			//return date1.Equals(date2);
			return (date1.Year == date2.Year &&
				date1.Month == date2.Month &&
				date1.Day == date2.Day &&
				date1.Hour == date2.Hour &&
				date1.Minute == date2.Minute &&
				date1.Second == date2.Second);
		}

		public override string Name 
		{
			get { return "DateTime"; }
		}

		public override string ToXML(object val) 
		{
			return ((DateTime)val).ToShortDateString();
		}

		public override object DeepCopyNotNull(object value) 
		{
			DateTime old = (DateTime) value;
			return new DateTime(old.Year, old.Month, old.Day, old.Hour, old.Minute, old.Second);
		}

		public override bool HasNiceEquals 
		{
			get { return true; }
		}

		public object StringToObject(string xml) 
		{
			return DateTime.Parse(xml);
		}

		public string ObjectToSQLString(object value) 
		{
			return "'" + value.ToString() + "'";
		}

		public object Next(object current) 
		{
			return Seed;
		}
		
		public object Seed 
		{
			get { return DateTime.Now; }
		}
	}
}
