using System;
using System.Data;

namespace NHibernate.Type {
	
	public class DoubleType : PrimitiveType {
		public override object Get(IDataReader rs, string name) {
			return rs[name];
		}

		public override System.Type PrimitiveClass {
			get { return typeof(double); }
		}

		public override System.Type ReturnedClass {
			get { return typeof(double); }
		}

		public override void Set(IDbCommand st, object value, int index) {
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.DbType = DbType.Double;
			parm.Value = value;
		}

		public override DbType SqlType {
			get { return DbType.Double; }
		}

		public override string Name {
			get { return "double"; }
		}

		public override string ObjectToSQLString(object value) {
			return value.ToString();
		}
	}
}
