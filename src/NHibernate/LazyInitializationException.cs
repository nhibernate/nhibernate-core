using System;

namespace NHibernate {
	/// <summary>
	/// A problem occurred trying to lazily initialize a collection or proxy (for example the session
	/// was closed) or iterate query results.
	/// </summary>
	public class LazyInitializationException : Exception {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LazyInitializationException));

		public LazyInitializationException(Exception root) : base("Hibernate lazy instantiation problem", root) {}

		public LazyInitializationException(string msg) : base(msg) {
			log.Error(msg, this);
		}

	}
}
