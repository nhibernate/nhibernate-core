using System;

namespace NHibernate {

	/// <summary>
	/// Throw when the user passes a transient instance to a <c>ISession</c> method that expects
	/// a persistent instance
	/// </summary>
	public class TransientObjectException : HibernateException {
		public TransientObjectException(string msg): base(msg) {	}
	}
}
