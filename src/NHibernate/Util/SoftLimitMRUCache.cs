using System;
using System.Collections;
using System.Runtime.Serialization;

namespace NHibernate.Util
{
	/// <summary> 
	/// Cache following a "Most Recently Used" (MRY) algorithm for maintaining a
	/// bounded in-memory size; the "Least Recently Used" (LRU) entry is the first
	/// available for removal from the cache.
	/// </summary>
	/// <remarks>
	/// This implementation uses a "soft limit" to the in-memory size of the cache,
	/// meaning that all cache entries are kept within a completely
	/// {@link java.lang.ref.SoftReference}-based map with the most recently utilized
	/// entries additionally kept in a hard-reference manner to prevent those cache
	/// entries soft references from becoming enqueued by the garbage collector.
	/// Thus the actual size of this cache impl can actually grow beyond the stated
	/// max size bound as long as GC is not actively seeking soft references for
	/// enqueuement.
	/// </remarks>
	[Serializable]
	public class SoftLimitMRUCache : IDeserializationCallback
	{
		private const int DefaultStrongRefCount = 128;
		private readonly object _syncRoot = new object();

		private readonly int strongReferenceCount;

		// actual cache of the entries.  soft references are used for both the keys and the
		// values here since the values pertaining to the MRU entries are kept in a
		// separate hard reference cache (to avoid their enqueuement/garbage-collection).
		[NonSerialized]
		private readonly IDictionary softReferenceCache = new WeakHashtable();

		// the MRU cache used to keep hard references to the most recently used query plans;
		// note : LRU here is a bit of a misnomer, it indicates that LRU entries are removed, the
		// actual kept entries are the MRU entries
		[NonSerialized]
		private LRUMap strongReferenceCache;

		public SoftLimitMRUCache(int strongReferenceCount)
		{
			this.strongReferenceCount = strongReferenceCount;
			strongReferenceCache = new LRUMap(strongReferenceCount);
		}

		public SoftLimitMRUCache()
			: this(DefaultStrongRefCount) {}

		#region IDeserializationCallback Members

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			strongReferenceCache = new LRUMap(strongReferenceCount);
		}

		#endregion

		public object this[object key]
		{
			get
			{
				lock (_syncRoot)
				{
					object result = softReferenceCache[key];
					if (result != null)
					{
						strongReferenceCache.Add(key, result);
					}
					return result;
				}
			}
		}

		public void Put(object key, object value)
		{
			lock (_syncRoot)
			{
				softReferenceCache[key] = value;
				strongReferenceCache[key] = value;
			}
		}

		public int Count
		{
			get
			{
				lock (_syncRoot)
				{
					return strongReferenceCache.Count;
				}
			}
		}

		public int SoftCount
		{
			get
			{
				lock (_syncRoot)
				{
					return softReferenceCache.Count;
				}
			}
		}

		public void Clear()
		{
			lock (_syncRoot)
			{
				strongReferenceCache.Clear();
				softReferenceCache.Clear();
			}
		}
	}
}
