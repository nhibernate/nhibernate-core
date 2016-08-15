using System;
using System.Runtime.CompilerServices;
using System.Threading;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

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
	public class SimpleMRUCache
#if FEATURE_SERIALIZATION
		: IDeserializationCallback
#endif
	{
		private const int DefaultStrongRefCount = 128;

		private object _syncRoot;

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

		private object SyncRoot
		{
			get
			{
				if (_syncRoot == null)
					Interlocked.CompareExchange(ref _syncRoot, new object(), null);

				return _syncRoot;
			}
		}

#if FEATURE_SERIALIZATION
		#region IDeserializationCallback Members

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			cache = new LRUMap(strongReferenceCount);
		}

		#endregion
#endif

		public object this[object key]
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				lock (SyncRoot)
				{
					return cache[key];
				}
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Put(object key, object value)
		{
			lock (SyncRoot)
			{
				cache.Add(key, value);
			}
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				lock (SyncRoot)
				{
					return cache.Count;
				}
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Clear()
		{
			lock (SyncRoot)
			{
				cache.Clear();
			}
		}
	}
}
