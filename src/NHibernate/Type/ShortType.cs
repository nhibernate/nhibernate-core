using System;
using System.Data;

namespace NHibernate.Type {
	
	public class ShortType : PrimitiveType, IDiscriminatorType, IVersionType {
		
		public override object Get(IDataReader rs, string name) {
			return rs[name];
		}
		public override System.Type PrimitiveClass {
			get { return typeof(short); }
		}
		public override System.Type ReturnedClass {
			get { return typeof(short); }
		}

		public override void Set(IDbCommand rs, object value, int index) {
			IDataParameter parm = rs.Parameters[index] as IDataParameter;
			parm.DbType = DbType.Int16;
			parm.Value = value;
		}

		public override DbType SqlType {
			get { return DbType.Int16; }
		}
		public override string Name {
			get { return "short"; }
		}
		public override string ObjectToSQLString(object value) {
			return value.ToString();
		}
		public object StringToObject(string xml) {
			return short.Parse(xml);
		}

		public object Next(object current) {
			return ((short)current) + 1;
		}
		public object Seed {
			get { return 0; }
		}
	}
}
