using System;
using System.Data;
using NHibernate.Util;


namespace NHibernate.Type {
	
	public class StringType : ImmutableType, IDiscriminatorType {
		

		public override object Get(IDataReader rs, string name) {
			return rs[name];
		}
		public override System.Type ReturnedClass {
			get { return typeof(string); }
		}
		public override void Set(IDbCommand st, object value, int index) {
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.DbType = DbType.String;
			parm.Value = value;
		}
		public override DbType SqlType {
			get { return DbType.String; }
		}
		public override string Name {
			get { return "string"; }
		}
		public string ObjectToSQLString(object value) {
			return "'" + (string) value + "'";
		}
		public object StringToObject(string xml) {
			return xml;
		}

		public override bool Equals(object x, object y) {
			return ObjectUtils.Equals(x, y);
		}

		public override string ToXML(object value) {
			return (string) value;
		}
	}
}
