using System;
using System.Data;

using NHibernate.Engine;

namespace NHibernate.Type {

	public class IntegerType : PrimitiveType, IDiscriminatorType, IVersionType {
	
		public override object Get(IDataReader rs, string name) {
            return rs[name];
		}

		public override System.Type PrimitiveClass {
			get { return typeof(int); }
		}

		public override System.Type ReturnedClass {
			get { return typeof(int); }
		}
		
		public override void Set(IDbCommand cmd, object value, int index) {
			IDataParameter parm = cmd.Parameters[index] as IDataParameter;
			parm.DbType = DbType.Int32;
			parm.Value = value;
		}

		public override DbType SqlType {
			get { return DbType.Int32; }
		}

		public override string Name {
			get { return "integer"; }
		}

		public override string ObjectToSQLString(object value) {
            return value.ToString();
		}

		public object StringToObject(string xml) {
            return int.Parse(xml);
		}

		public virtual object Next(object current) {
			return ((int) current) + 1;
		}

		public virtual object Seed {
			get	{ return 0; }
		}
	}
}