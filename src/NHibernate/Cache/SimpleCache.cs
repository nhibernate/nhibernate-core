using System;
using System.Collections;

namespace NHibernate.Cache {

	/// <summary>
	/// A simple <c>Hashtable</c> based cache
	/// </summary>
	public class SimpleCache : ICache {
		private Hashtable cache = new Hashtable();

		public object this[object key] {
			get { return cache[key]; }
			set { cache[key] = value; }
		}
	}
}
