using System;

namespace NHibernate.Odmg {
	/// <summary>
	/// This is the base class for all exceptions thrown by an ODMG implementation.
	/// Translated from Jakarta OJB project
	/// Author: edgar.sanchez@objeq.com
	/// Version: ODMG 3.0
	/// </summary>
	public class ODMGException : Exception {
		/// <summary>
		/// Construct an ODMGException object without an error message.
		/// </summary>
		public ODMGException() : base() {
		}

		/// <summary>
		/// Construct an ODMGException object with an error message.
		/// </summary>
		/// <param name="msg">The error message associated with this exception.</param>
		public ODMGException(string msg) : base(msg) {
		}
	}
}

