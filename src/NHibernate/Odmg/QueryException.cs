using System;

namespace NHibernate.Odmg
{
	/// <summary>
	/// This is the base class for all exceptions associated with queries.
	/// Translated from Jakarta OJB project
	/// Author: edgar.sanchez@objeq.com
	/// Version: ODMG 3.0
	/// </summary>
	public class QueryException : ODMGException {
		/// <summary>
		/// Construct an QueryException object without an error message.
		/// </summary>
		public QueryException() : base() {
		}

		/// <summary>
		/// Construct an QueryException object with an error message.
		/// </summary>
		/// <param name="msg">The error message associated with this exception.</param>
		public QueryException(string msg) : base(msg) {
		}
	}
}
