using System;
using System.Data;

namespace NHibernate {
	/// <summary>
	/// Wraps an <c>DataException</c>. Indicates that an exception occurred during an ADO.NET call.
	/// </summary>
	public class ADOException : HibernateException {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ADOException));
		private Exception sqle;

		public ADOException(DataException root) : this("DataException occurred", root) { }

		public ADOException(string str, Exception root) : base(str, root) {
			sqle = root;
			log.Error(str, root); 
		}

	}
}
