using System;
using System.Data;

using NHibernate.Sql;

namespace NHibernate.Type {

	/// <summary>
	/// BooleanType
	/// </summary>

	//Had to use setShort / getShort instead of setBoolean / getBoolean
	//to work around a HypersonicSQL driver bug

	public class BooleanType : PrimitiveType, IDiscriminatorType {

		private static readonly string TRUE = "1";
		private static readonly string FALSE = "0";

		public override object Get(IDataReader rs, string name) {
			return (bool)rs[name];
		}

		public override System.Type PrimitiveClass {
			get { return typeof(bool); }
		}
	
		public override System.Type ReturnedClass {
			get { return typeof(bool); }
		}

		public override void Set(IDbCommand cmd, object value, int index) {
			( (IDataParameter)cmd.Parameters[index] ).Value = (bool) value;
		}

		public override DbType SqlType {
			get { return DbType.Boolean; }
		}
	
		public override string Name {
			get { return "boolean"; }
		}
	
		public override string ObjectToSQLString(object value) {
			return ( (bool)value ) ? TRUE : FALSE;
		}
	
		public virtual object StringToObject(string xml) {
			return bool.Parse(xml);
		}
	}
}