using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type {
	
	public class DoubleType : PrimitiveType {
		
		internal DoubleType(DoubleSqlType sqlType) : base(sqlType) {
		}

		public override object Get(IDataReader rs, int index) {
			return rs.GetDouble(index);
		}

		public override object Get(IDataReader rs, string name) {
			return Get(rs, rs.GetOrdinal(name));
		}

		public override System.Type PrimitiveClass {
			get { return typeof(double); }
		}

		public override System.Type ReturnedClass {
			get { return typeof(double); }
		}

		public override void Set(IDbCommand st, object value, int index) {
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.Value = value;
		}

		public override string Name {
			get { return "Double"; }
		}

		public override string ObjectToSQLString(object value) {
			return value.ToString();
		}
	}
}
