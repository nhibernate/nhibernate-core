using System;

namespace NHibernate {

	/// <summary>
	/// A problem occurred translating a Hibernate query to SQL due to invalid query syntax, etc.
	/// </summary>
	public class QueryException : HibernateException {
		private string queryString;

		public QueryException(string msg) : base(msg) {}

		public QueryException(string msg, Exception e) : base(msg, e) {}

		public QueryException(Exception e) : base(e) {}

		public string QueryString {
			get { return queryString; }
			set { queryString = value; }
		}

		public override string Message {
			get { return base.Message + " [" + queryString + "]"; }
		}


	}
}
