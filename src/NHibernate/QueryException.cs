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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public QueryException( string message ) : base( message )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		public QueryException( string message, Exception e ) : base( message, e )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public QueryException( Exception e ) : base( e )
		{
		}

		/// <summary></summary>
		public string QueryString
		{
			get { return queryString; }
			set { queryString = value; }
		}

		/// <summary></summary>
		public override string Message
		{
			get { return base.Message + " [" + queryString + "]"; }
		}
	}
}