using System;

namespace NHibernate {
	
	/// <summary>
	/// Any exception that occurs in the O-R persistence layer.
	/// </summary>
	/// <remarks>Exceptions that occur in the database layer are left as native exceptions</remarks>
	public class HibernateException : ApplicationException {
		
		public HibernateException(Exception e) : base(string.Empty, e) { }

		public HibernateException(string str, Exception e) : base(str, e) { }

		public HibernateException(string str) : base(str) { }
	}
}
