using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type {
	
	/// <summary>
	/// Maps a System.DateTime Property to a column that stores date & time.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The value stored in the database depends on what your Data Provider is capable
	/// of storing.  So there is a possibility that the DateTime you save will not be
	/// the same DateTime you get back when you check DateTime.Equals(DateTime) because
	/// they will have their milliseconds off.
	/// </p>  
	/// <p>
	/// For example - MsSql Server 2000 is only accurate to 3.33 milliseconds.  So if 
	/// NHibernate writes a value of '01/01/98 23:59:59.995' to the Prepared Command, MsSql
	/// will store it as '1998-01-01 23:59:59.997'.
	/// </p>
	/// <p>
	/// Please review the documentation of your Database server.
	/// </p>
	/// 
	/// </remarks>
	public class DateTimeType : MutableType, IIdentifierType, ILiteralType 
	{
		
		public DateTimeType(DateTimeSqlType sqlType) : base(sqlType) {
		}

		public override object Get(IDataReader rs, int index) {
			return rs.GetDateTime(index);
		}

		public override object Get(IDataReader rs, string name) {
			return Get(rs,rs.GetOrdinal(name));// rs.[name];
		}

		public override System.Type ReturnedClass {
			get { return typeof(DateTime); }
		}
		public override void Set(IDbCommand st, object value, int index) {
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.DbType = DbType.DateTime;
			//TODO: figure out if this is a good solution for NULL DATES
			if(value.Equals(System.DateTime.MinValue)) {
				parm.Value = DBNull.Value;
			}
			else {
				parm.Value = value;
			}
		}

		public override bool Equals(object x, object y) {
			if (x==y) return true;
			// DateTime can't be null because it is a struct - so comparing 
			// them this way is useless - instead use the magic number...
			//if (x==null || y==null) return false;

			DateTime date1 = (x==null)? DateTime.MinValue : (DateTime) x;
			DateTime date2 = (y==null)? DateTime.MinValue : (DateTime) y;

			return date1.Equals(date2);
		}
		public override string Name {
			get { return "DateTime"; }
		}
		public override string ToXML(object val) {
			return ((DateTime)val).ToShortDateString();
		}
		public override object DeepCopyNotNull(object value) {
			DateTime old = (DateTime) value;
			return new DateTime(old.Year, old.Month, old.Day, old.Hour, old.Minute, old.Second, old.Millisecond);
		}
		public override bool HasNiceEquals {
			get { return true; }
		}
		public object StringToObject(string xml) {
			return DateTime.Parse(xml);
		}
		public string ObjectToSQLString(object value) {
			return "'" + value.ToString() + "'";
		}
	}
}
