using System;

namespace NHibernate.Type {

	/// <summary>
	/// IntegerType.
	/// </summary>
	public class IntegerType : PrimitiveType, IDiscriminatorType, IVersionType {
	
		// In C# boxing and unboxing is automatic. No System.Number.Integer is needed.
		// Is it correct?

		/*
		public object Get(ResultSet rs, string name) {
            return new Integer(rs.getInt(name));
		}
		*/

		public override System.Type PrimitiveClass {
			get { return typeof(int); }
		}

		public System.Type ReturnedClass {
			get { return typeof(int); }
		}
		
		/*
		public void Set(PreparedStatement st, object val, int index) {
				st.SetInt(index, (int) val);
		}
		*/

		public override Sql.Types SqlType {
			get { return Sql.Types.Integer; }
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
		
		public object Next(object current) {
			return ((int) current) + 1;
		}

		public object Seed {
			get	{ return 0; }
		}
	}
}