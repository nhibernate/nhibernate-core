using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Odmg;

namespace NHibernate.Collection {

	/// <summary>
	/// Last modified by edgar.sanchez@objeq.com
	/// </summary>
	[Serializable]
	public abstract class ODMGCollection : PersistentCollection, IDCollection {
		
		public ODMGCollection(ISessionImplementor session) : base(session) {}
	
		public bool ExistsElement(string queryString) {
			return ((ICollection)session.Filter(this, queryString)).Count > 0;
		}

		public IDCollection Query(string queryString) {
			try {
				return new List( session, (IList) session.Filter(this, queryString) );
			} catch(HibernateException he) {
				throw new QueryInvalidException(he.Message);
			}
		}

		public IEnumerator Select(string queryString) {
			try {
				return ((ICollection)session.Filter(this, queryString)).GetEnumerator();
			} catch(HibernateException he) {
				throw new QueryInvalidException(he.Message);
			}
		}

		public object SelectElement(string queryString) {
//			foreach(object obj in Select(queryString)) {
//				return obj;
//			}
//			return null;

			IEnumerator iter = Select(queryString);
			return iter.MoveNext() ? iter.Current : null;
		}

		public abstract void CopyTo(Array a, int index);
		public abstract int Count { get; }
		public abstract bool IsSynchronized { get; }
		public abstract object SyncRoot { get; }
		public abstract IEnumerator GetEnumerator();

	}
}
