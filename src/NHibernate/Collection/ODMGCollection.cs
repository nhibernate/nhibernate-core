using System;
using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Collection {

	public abstract class ODMGCollection : PersistentCollection {
		
		public ODMGCollection(ISessionImplementor session) : base(session) {}
	
		public bool ExistsElement(string queryString) {
			return Select(queryString).Count > 0;
		}

		public List Query(string queryString) {
			try {
				return new List( session, (IList) session.Filter(this, queryString) );
			} catch(HibernateException he) {
				throw he;
			}
		}

		public ICollection Select(string queryString) {
			try {
				return session.Filter(this, queryString);
			} catch(HibernateException he) {
				throw he;
			}
		}

		public object SelectElement(string queryString) {
			foreach(object obj in Select(queryString)) {
				return obj;
			}
			return null;
		}
	}
}
