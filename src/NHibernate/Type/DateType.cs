using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Type {
	
	public class DateType : MutableType, IIdentifierType, ILiteralType, IVersionType {


	    public DateType (DateSqlType sqlType) : base(sqlType)
	    {
	    }

	    public override object Get(IDataReader rs, int index) 
		{
			DateTime dbValue = rs.GetDateTime(index);
			return new DateTime(dbValue.Year, dbValue.Month, dbValue.Day);
		}

		public override object Get(IDataReader rs, string name) 
		{
			return Get(rs, rs.GetOrdinal(name));// rs.[name];
		}

		public override System.Type ReturnedClass {
			get { return typeof(DateTime); }
		}

		public override void Set(IDbCommand st, object value, int index) {
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			if((DateTime)value<new DateTime(1753,1,1))
			{
				parm.Value = DBNull.Value;
			}
			else 
			{

				parm.DbType = DbType.Date;
				parm.Value = value;
			}
		}

		public override bool Equals(object x, object y) {
			if (x==y) return true;
			if (x==null || y==null) return false;

			DateTime date1 = (DateTime) x;
			DateTime date2 = (DateTime) y;

			return date1.Day == date2.Day
				&& date1.Month == date2.Month 
				&& date1.Year == date2.Year;
		}

		public override string Name {
			get { return "Date"; }
		}

		public override string ToXML(object val) {
			return ((DateTime)val).ToShortDateString();
		}

		public override object DeepCopyNotNull(object value) {
			DateTime old = (DateTime) value;
			return new DateTime(old.Year, old.Month, old.Day);
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
