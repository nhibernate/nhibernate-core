using System;
using System.Data;
using NHibernate.Util;

namespace NHibernate.Type {
	
	public class ClassType : ImmutableType {

		public override object Get(IDataReader rs, string name) {
			string str = (string) NHibernate.String.Get(rs, name);
			if (str == null) {
				return null;
			}
			else {
				try {
					return ReflectHelper.ClassForName(str);
				}
				catch (TypeLoadException) {
					throw new HibernateException("Class not found: " + str);
				}
			}
		}

		public override void Set(IDbCommand cmd, object value, int index) {
			NHibernate.String.Set(cmd, ( (System.Type) value ).Name, index);
		}
	
		public override DbType SqlType {
			get { return NHibernate.String.SqlType; }
		}
	
		public override string ToXML(object value) {
			return ( (System.Type) value ).Name;
		}
	
		public override System.Type ReturnedClass {
			get { return typeof(System.Type); }
		}
	
		public override bool Equals(object x, object y) {
			return (x==y); //???
		}
	
		public override string Name {
			get { return "class"; }
		}
	}
}