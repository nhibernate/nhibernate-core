using System;

namespace NHibernate 
{
	/// <summary>
	/// A problem occurred translating a Hibernate query to SQL due to invalid query syntax, etc.
	/// </summary>
	[Serializable]
	public class QueryException : HibernateException 
	{
		private string queryString;

		public QueryException(string message) : base(message) {}

		public QueryException(string message, Exception e) : base(message, e) {}

		public QueryException(Exception e) : base(e) {}

		public string QueryString 
		{
			get { return queryString; }
			set { queryString = value; }
		}

		public override string Message 
		{
			get { return base.Message + " [" + queryString + "]"; }
		}
	}
}
