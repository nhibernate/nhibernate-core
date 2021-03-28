using System;
using System.Runtime.Serialization;

namespace NHibernate.Util
{
	/// <summary> 
	/// Cache following a "Most Recently Used" (MRU) algorithm for maintaining a
	/// bounded in-memory size; the "Least Recently Used" (LRU) entry is the first
	/// available for removal from the cache.
	/// </summary>
	/// <remarks>
	/// This implementation uses a bounded MRU Map to limit the in-memory size of
	/// the cache.  Thus the size of this cache never grows beyond the stated size. 
	/// </remarks>
	[Serializable]
	public class SimpleMRUCache : IDeserializationCallback
	{
		private const int DefaultStrongRefCount = 128;

		private readonly object _syncRoot = new object();

		private readonly int strongReferenceCount;

		[NonSerialized]
		private LRUMap cache;

		public SimpleMRUCache()
			: this(DefaultStrongRefCount) {}

		public SimpleMRUCache(int strongReferenceCount)
		{
			this.strongReferenceCount = strongReferenceCount;
			cache = new LRUMap(strongReferenceCount);
		}

		#region IDeserializationCallback Members

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			cache = new LRUMap(strongReferenceCount);
		}

		#endregion

		public object this[object key]
		{
			get
			{
				lock (_syncRoot)
				{
					return cache[key];
				}
			}
		}

		public void Put(object key, object value)
		{
			lock (_syncRoot)
			{
				cache.Add(key, value);
			}
		}

		public int Count
		{
			get
			{
				lock (_syncRoot)
				{
					return cache.Count;
				}
			}
		}

		public void Clear()
		{
			lock (_syncRoot)
			{
				cache.Clear();
			}
		}
	}
}
