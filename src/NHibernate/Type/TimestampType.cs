using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type {
	
	/// <summary>
	/// This is almost the exact same type as the DateTime except it can be used
	/// in the version column.  It is not a SQL Server "timestamp" column.
	/// </summary>
	public class TimestampType : MutableType, IVersionType, ILiteralType{
		
		public TimestampType(DateTimeSqlType sqlType) : base(sqlType) {
		}

		public override object Get(IDataReader rs, int index) {
			return rs.GetDateTime(index);
		}

		public override object Get(IDataReader rs, string name) {
			return Get(rs, rs.GetOrdinal(name));// rs.[name];
		}

		public override System.Type ReturnedClass {
			get { return typeof(DateTime); }
		}
		public override void Set(IDbCommand st, object value, int index) {
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.Value = value;
		}

		public override string Name {
			get { return "Timestamp"; }
		}
		public override string ToXML(object val) {
			return ((DateTime)val).ToShortTimeString();
		}
		public override object DeepCopyNotNull(object value) {
			return value;
		}

		public override bool Equals(object x, object y) {
			if (x==y) return true;
			if (x==null || y==null) return false;

			long xTime = ((DateTime)x).Ticks;
			long yTime = ((DateTime)y).Ticks;
			return xTime == yTime; //TODO: Fixup
		}

		public override bool HasNiceEquals {
			get { return true; }
		}

		public object Next(object current) {
			return Seed;
		}
		
		public object Seed {
			get { return DateTime.Now; }
		}

		public string ObjectToSQLString(object value) {
			return "'" + value.ToString() + "'";
		}
	}
}
