using System;

namespace NHibernate {
	/// <summary>
	/// An exception that usually occurs at configuration time, rather than runtime, as a result of
	/// something screwy in the O-R mappings
	/// </summary>
	public class MappingException : HibernateException {
		
		public MappingException(string msg, Exception root) : base(msg, root) {}

		public MappingException(Exception root) : base(root) { }

		public MappingException(string msg) : base(msg) {}
	}
}
