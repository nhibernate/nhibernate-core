using System;
using System.Data;

namespace NHibernate.Type {

	/// <summary>
	/// DecimalType
	/// </summary>
	public class DecimalType : PrimitiveType, IIdentifierType, IVersionType {
	
		public override object Get(IDataReader rs, string name) {
			return rs[name];
		}

		public override System.Type PrimitiveClass {
			get { return typeof(Decimal); }
		}

		public override System.Type ReturnedClass {
			get { return typeof(Decimal); }
		}

		public override void Set(IDbCommand st, object value, int index) {
			IDataParameter parm = st.Parameters[index] as IDataParameter;
			parm.DbType = DbType.Decimal;
			parm.Value = value;
		}

		public override DbType SqlType {
			get { return DbType.Decimal; }
		}

		public override string Name {
			get { return "decimal"; }
		}

		public object StringToObject(string xml) {
			return long.Parse(xml);
		}

		public object Next(object current) {
			return ((Decimal)current) + 1;
		}

		public object Seed {
			get { return 0; }
		}

		public override string ObjectToSQLString(object value) {
			return value.ToString();
		}
	}
}