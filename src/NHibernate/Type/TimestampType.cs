using System;
using System.Data;

namespace NHibernate.Type {
	
	public class TimestampType : MutableType, IVersionType, ILiteralType{
		
		public override object Get(IDataReader rs, string name) {
			return rs[name];
		}
		public override System.Type ReturnedClass {
			get { return typeof(DateTime); }
		}
		public override void Set(IDbCommand st, object value, int index) {
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.DbType = DbType.DateTime;
			parm.Value = value;
		}

		public override DbType SqlType {
			get { return DbType.DateTime; }
		}
		public override string Name {
			get { return "timestamp"; }
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
