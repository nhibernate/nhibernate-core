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
			/*
			try {
				return new List( session, (IList) session.Filter(this, queryString) );
			} catch(HibernateException he) {
				throw he;
			}
			*/
			//TODO: uncomment
			return null;
		}

		public ICollection Select(string queryString) {
			try {
				return session.Filter(this, queryString);
			} catch(HibernateException he) {
				throw he;
			}
		}

		public object SelectElement(string queryString) {
			ICollection coll = Select(queryString);
			foreach(object obj in coll) {
				return obj;
			}
			return null;
		}
	}
}
