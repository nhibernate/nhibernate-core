using System;
using System.Data;

namespace NHibernate.Type {
	
	public class FloatType : PrimitiveType {
		
		public override object Get(IDataReader rs, string name) {
			return rs[name];
		}

		public override System.Type PrimitiveClass {
			get { return typeof(float); }
		}

		public override System.Type ReturnedClass {
			get { return typeof(float); }
		}

		public override void Set(IDbCommand st, object value, int index) {
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.DbType = DbType.Single;
			parm.Value = value;
		}

		public override DbType SqlType {
			get { return DbType.Single; }
		}

		public override string Name {
			get { return "float"; }
		}

		public override string ObjectToSQLString(object value) {
			return value.ToString();
		}
	}
}
