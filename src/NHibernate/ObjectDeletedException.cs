using System;

namespace NHibernate {

	/// <summary>
	/// Thrown when the user tries to pass a deleted object to the <c>ISession</c>.
	/// </summary>
	public class ObjectDeletedException : HibernateException {
		private object identifier;

		public ObjectDeletedException(string msg, object identifier) : base(msg) {
			this.identifier = identifier;
		}
		public object Identifier {
			get { return identifier; }
		}
		public override string Message {
			get { return base.Message + ": " + identifier; }
		}
	}
}
