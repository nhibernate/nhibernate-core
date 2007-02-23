using System;
using System.Collections;

using Bamboo.Prevalence;

namespace NHibernate.Caches.Prevalence
{
	/// <summary>
	/// Summary description for CacheSystem.
	/// </summary>
	[Serializable]
	public sealed class CacheSystem : MarshalByRefObject
	{
		private Hashtable _items;

		/// <summary>
		/// default constructor
		/// </summary>
		public CacheSystem()
		{
			_items = new Hashtable();
		}

		/// <summary>
		/// retrieve the value for the given key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object Get(object key)
		{
			if (key == null) return null;

			CacheEntry entry = _items[key] as CacheEntry;
			if (entry == null) return null;

			return entry.Value;
		}

		/// <summary>
		/// add or update an object in the cache
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Add(object key, object value)
		{
			if (key == null) return;

			CacheEntry entry = _items[key] as CacheEntry;
			if (entry == null)
			{
				entry = new CacheEntry();
				entry.Key = key;
				entry.Value = value;
				entry.DateCreated = PrevalenceEngine.Now;
				_items.Add(key, entry);
			}
			else
			{
				entry.Value = value;
				_items[key] = entry;
			}
		}

		/// <summary>
		/// remove an item from the cache
		/// </summary>
		/// <param name="key"></param>
		public void Remove(object key)
		{
			_items.Remove(key);
		}

		/// <summary>
		/// clear the cache
		/// </summary>
		public void Clear()
		{
			_items.Clear();
		}
	}
}