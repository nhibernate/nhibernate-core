using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type {
	
	public class Int64Type : PrimitiveType, IIdentifierType, IVersionType {
		
		internal Int64Type() : base( new Int64SqlType() ) 
		{
		}

		public override object Get(IDataReader rs, int index) {
			return Convert.ToInt64(rs[index]);
		}

		public override object Get(IDataReader rs, string name) {
			return Convert.ToInt64(rs[name]);
		}

		public override System.Type PrimitiveClass {
			get { return typeof(System.Int64); }
		}

		public override System.Type ReturnedClass {
			get { return typeof(System.Int64); }
		}

		public override void Set(IDbCommand st, object value, int index) {
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.Value = value;
		}

		public override string Name {
			get { return "Int64"; }
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
