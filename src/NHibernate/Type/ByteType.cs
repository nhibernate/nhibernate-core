using System;
using System.Data;

using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Type {

	/// <summary>
	/// ByteType.
	/// </summary>
	public class ByteType : PrimitiveType, IDiscriminatorType {
		
		internal ByteType(ByteSqlType sqlType) : base(sqlType) {
		}

		public override object Get(IDataReader rs, int index) {
			return rs.GetByte(index);
		}

		public override object Get(IDataReader rs, string name) {
			return Get(rs, rs.GetOrdinal(name));
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

		public override string Name {
			get { return "Byte"; }
		}

		public override string ObjectToSQLString(object value) {
			return value.ToString();
		}

		public virtual object StringToObject(string xml) {
			return byte.Parse(xml);
		}
	}
}