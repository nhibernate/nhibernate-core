using System;
using System.Data;

namespace NHibernate.Type {
	
	public class LongType : PrimitiveType, IIdentifierType, IVersionType {
		
		public override object Get(IDataReader rs, string name) {
			return rs[name];
		}

		public override System.Type PrimitiveClass {
			get { return typeof(long); }
		}

		public override System.Type ReturnedClass {
			get { return typeof(long); }
		}

		public override void Set(IDbCommand st, object value, int index) {
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.DbType = DbType.Int64;
			parm.Value = value;
		}

		public override DbType SqlType {
			get { return DbType.Int64; }
		}

		public override string Name {
			get { return "long"; }
		}

		public object StringToObject(string xml) {
			return long.Parse(xml);
		}

		public object Next(object current) {
			return ((long)current) + 1;
		}

		public object Seed {
			get { return 0; }
		}

		public override string ObjectToSQLString(object value) {
			return value.ToString();
		}
	}
}
