using System;

namespace NHibernate {
	/// <summary>
	/// Thrown if Hibernate can't instantiate an entity or component class at runtime.
	/// </summary>
	public class InstantiationException : HibernateException {
		private System.Type type;

		public InstantiationException(string s, System.Type type, Exception root) : base(s, root) {
			this.type = type;
		}
		public System.Type PersistentType {
			get { return type; }
		}
		public override string Message {
			get { return base.Message + type.FullName; }
		}
	}
}
