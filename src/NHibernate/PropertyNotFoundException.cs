using System;

namespace NHibernate {

	/// <summary>
	/// Indicates that an expected getter or setter method could not be found on a class
	/// </summary>
	public class PropertyNotFoundException : MappingException {
		public PropertyNotFoundException(string msg, Exception root) : base(msg, root) {}
		public PropertyNotFoundException(Exception root) : base(root) {}
		public PropertyNotFoundException(string msg) : base(msg) {}
	}
}
