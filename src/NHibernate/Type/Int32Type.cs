using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Type {

	public class Int32Type : PrimitiveType, IDiscriminatorType, IVersionType {
	
		internal Int32Type() : base( new Int32SqlType() ) 
		{
		}

		public override object Get(IDataReader rs, int index) {
			return Convert.ToInt32(rs[index]);
		}

		public override object Get(IDataReader rs, string name) {
			return Convert.ToInt32(rs[name]);
		}

		public override System.Type PrimitiveClass {
			get { return typeof(System.Int32); }
		}

		public override System.Type ReturnedClass {
			get { return typeof(System.Int32); }
		}
		
		public override void Set(IDbCommand cmd, object value, int index) {
			IDataParameter parm = cmd.Parameters[index] as IDataParameter;
			parm.Value = value;
		}

		public override string Name {
			get { return "Int32"; }
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