using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

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
	public class SoftLimitMRUCache<TKey, TValue> : IDeserializationCallback where TKey : class where TValue : class
	{
		private const int DefaultStrongRefCount = 128;
		private object _syncRoot;

		private readonly int _strongReferenceCount;

		// actual cache of the entries.  soft references are used for both the keys and the
		// values here since the values pertaining to the MRU entries are kept in a
		// separate hard reference cache (to avoid their enqueuement/garbage-collection).
		[NonSerialized]
		private readonly IDictionary<TKey, TValue> _softReferenceCache = new WeakHashtable<TKey, TValue>();

		// the MRU cache used to keep hard references to the most recently used query plans;
		// note : LRU here is a bit of a misnomer, it indicates that LRU entries are removed, the
		// actual kept entries are the MRU entries
		[NonSerialized]
		private LRUMap _strongReferenceCache;

		public SoftLimitMRUCache(int strongReferenceCount)
		{
			_strongReferenceCount = strongReferenceCount;
			_strongReferenceCache = new LRUMap(strongReferenceCount);
		}

		public SoftLimitMRUCache()
			: this(DefaultStrongRefCount) { }

		private object SyncRoot
		{
			get
			{
				if (_syncRoot == null)
					Interlocked.CompareExchange(ref _syncRoot, new object(), null);

				return _syncRoot;
			}
		}

		#region IDeserializationCallback Members

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			_strongReferenceCache = new LRUMap(_strongReferenceCount);
		}

		#endregion

		public TValue this[TKey key]
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				lock (SyncRoot)
				{
					if (_softReferenceCache.TryGetValue(key, out var result))
					{
						_strongReferenceCache.Add(key, result);
					}
					return result;
				}
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Put(TKey key, TValue value)
		{
			lock (SyncRoot)
			{
				_softReferenceCache[key] = value;
				_strongReferenceCache[key] = value;
			}
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				lock (SyncRoot)
				{
					return _strongReferenceCache.Count;
				}
			}
		}

		public int SoftCount
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				lock (SyncRoot)
				{
					return _softReferenceCache.Count;
				}
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Clear()
		{
			lock (SyncRoot)
			{
				_strongReferenceCache.Clear();
				_softReferenceCache.Clear();
			}
		}
	}
}
