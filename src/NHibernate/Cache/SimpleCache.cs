using System;
using System.Collections;

namespace NHibernate.Cache {

	/// <summary>
	/// A simple <c>Hashtable</c> based cache
	/// </summary>
	public class SimpleCache : ICache {
		private Hashtable cache = new Hashtable();

		public object this[object key] {
			get {
				WeakReference wr = cache[key] as WeakReference;
				if (wr == null || !wr.IsAlive) {
					cache[key] = null;
					return null;
				}
				return wr.Target; 
			}
			set { 
				cache[key] = new WeakReference(value);
			}
		}
	}
}
