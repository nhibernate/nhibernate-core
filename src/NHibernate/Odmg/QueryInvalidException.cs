using System;

namespace NHibernate.Odmg
{
	/// <summary>
	/// This exception is thrown if the query is not a valid OQL query.
	/// Translated from Jakarta OJB project
	/// Author: edgar.sanchez@objeq.com
	/// Version: ODMG 3.0
	/// </summary>
	public class QueryInvalidException : ODMGException {
		/// <summary>
		/// Construct an QueryInvalidException object without an error message.
		/// </summary>
		public QueryInvalidException() : base() {
		}

		/// <summary>
		/// Construct an QueryInvalidException object with an error message.
		/// </summary>
		/// <param name="msg">A string indicating why the OQLQuery instance does not
		/// represent a valid OQL query.</param>
		public QueryInvalidException(string msg) : base(msg) {
		}
	}
}
