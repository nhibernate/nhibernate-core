using System;
using System.Data;

using NHibernate.Engine;

namespace NHibernate.Type {

	/// <summary>
	/// IntegerType.
	/// </summary>
	public class IntegerType : PrimitiveType, IDiscriminatorType, IVersionType {
	
		// In C# boxing and unboxing is automatic. No System.Number.Integer is needed.
		// Is it correct?

		public override object Get(IDataReader rs, string name) {
            return (int)rs[name];

			//For performance reason should be better read cursor by int index
			//ie  return re.GetInt32(index);
		}

		public override System.Type PrimitiveClass {
			get { return typeof(int); }
		}

		public override System.Type ReturnedClass {
			get { return typeof(int); }
		}
		
		public override void Set(IDbCommand cmd, object value, int index) {
			( (IDataParameter)cmd.Parameters[index] ).Value = (int) value;
		}

		public override Sql.Types SqlType {
			get { return Sql.Types.Integer; }
		}

		public override string Name {
			get { return "integer"; }
		}

		public override string ObjectToSQLString(object value) {
            return value.ToString();
		}

		public virtual object StringToObject(string xml) {
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