using System;
using System.Data;

using NHibernate.Engine;

namespace NHibernate.Type {

	/// <summary>
	/// ByteType.
	/// </summary>
	public class ByteType : PrimitiveType, IDiscriminatorType {
		
		public override object Get(IDataReader rs, string name) {
			return (byte)rs[name];
		}

		public override System.Type PrimitiveClass {
			get { return typeof(byte); }
		}

		public override System.Type ReturnedClass {
			get { return typeof(byte); }
		}
		
		public override void Set(IDbCommand cmd, object value, int index) {
			( (IDataParameter)cmd.Parameters[index] ).Value = (byte) value;
		}

		public override DbType SqlType {
			get { return DbType.Byte; }
		}

		public override string Name {
			get { return "byte"; }
		}

		public override string ObjectToSQLString(object value) {
			return value.ToString();
		}

		public virtual object StringToObject(string xml) {
			return byte.Parse(xml);
		}
	}
}