using System;

namespace NHibernate {
	
	public class CallbackException : HibernateException {
		
		public CallbackException(Exception root) : base("An exception occured in a callback", root) {}

		public CallbackException(string message) : base(message) {}

		public CallbackException(string message, Exception e) : base(message, e) {}
	}
}
