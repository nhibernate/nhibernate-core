using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type {
	
	public class SingleType : PrimitiveType {
		
		internal SingleType(SingleSqlType sqlType) : base(sqlType) {
		}

		public override object Get(IDataReader rs, int index) {
			return rs.GetFloat(index);
		}

		public override object Get(IDataReader rs, string name) {
			return Get(rs, rs.GetOrdinal(name));
		}

		public override System.Type PrimitiveClass {
			get { return typeof(System.Single); }
		}

		public override System.Type ReturnedClass {
			get { return typeof(System.Single); }
		}

		public override void Set(IDbCommand st, object value, int index) {
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.Value = value;
		}

		public override string Name {
			get { return "Single"; }
		}

		public override string ObjectToSQLString(object value) {
			return value.ToString();
		}
	}
}
