using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type {
	
	public class Int16Type : PrimitiveType, IDiscriminatorType, IVersionType {
		
		internal Int16Type(Int16SqlType sqlType) : base(sqlType) {
		}

		public override object Get(IDataReader rs, int index) {
			return rs.GetInt16(index);
		}

		public override object Get(IDataReader rs, string name) {
			return Get(rs, rs.GetOrdinal(name));
		}
		public override System.Type PrimitiveClass {
			get { return typeof(System.Int16); }
		}
		public override System.Type ReturnedClass {
			get { return typeof(System.Int16); }
		}

		public override void Set(IDbCommand rs, object value, int index) {
			IDataParameter parm = rs.Parameters[index] as IDataParameter;
			parm.Value = value;
		}

		public override string Name {
			get { return "Int16"; }
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
