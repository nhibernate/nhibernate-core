using System;

namespace NHibernate {

	/// <summary>
	/// Indicates failure of an assertion: a possible bug in NHibernate
	/// </summary>
	public class AssertionFailure : ApplicationException {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AssertionFailure));

		public AssertionFailure(string s) : base(s) {
			log.Error("An AssertionFailure occured - this may indicate a bug in NHibernate", this);
		}

		public AssertionFailure(string s, Exception e) : base(s, e) {
			log.Error("An AssertionFailure occured - this may indicate a bug in NHibernate", e);
		}
	}
}
