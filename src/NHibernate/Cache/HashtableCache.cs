using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace NHibernate.Cache {

	/// <summary>
	/// A simple <c>Hashtable</c> based cache
	/// </summary>
	public class HashtableCache : ICache
	{
		private static object synchObject = new object();
		private Hashtable cache = new Hashtable();
		private string region;

		public HashtableCache(string region) 
		{
			this.region = region;
		}

		#region ICache Members
		
		public object Get(object key) 
		{
			return cache[key];
		}

		public void Put(object key, object value) 
		{
			cache[key] = value;
		}

		public void Remove(object key)
		{
			cache.Remove(key);
		}

		public void Clear()
		{
			cache.Clear();
		}

		/// <summary>
		/// Destroys the existing cache by setting it to a new Hashtable.
		/// </summary>
		public void Destroy()
		{
			cache = new Hashtable();
		}

		public string Region
		{
			set	{ region = value; }
		}


		#endregion
	}
}
