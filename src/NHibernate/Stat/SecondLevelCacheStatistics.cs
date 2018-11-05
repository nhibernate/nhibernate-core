using System;
using System.Collections;
using System.Text;
using NHibernate.Cache;

namespace NHibernate.Stat
{
	/// <summary> Second level cache statistics of a specific region </summary>
	[Serializable]
	public class SecondLevelCacheStatistics : CategorizedStatistics
	{
		[NonSerialized]
		// 6.0 TODO: type as CacheBase instead
#pragma warning disable 618
		private readonly ICache cache;
#pragma warning restore 618
		internal long hitCount;
		internal long missCount;
		internal long putCount;

		// 6.0 TODO: get as CacheBase instead
#pragma warning disable 618
		public SecondLevelCacheStatistics(ICache cache) : base(cache.RegionName)
#pragma warning restore 618
		{
			this.cache = cache;
		}

		public long HitCount
		{
			get { return hitCount; }
		}

		public long MissCount
		{
			get { return missCount; }
		}

		public long PutCount
		{
			get { return putCount; }
		}

		/// <summary>
		/// Not ported yet
		/// </summary>
		public long ElementCountInMemory
		{
			get
			{
				return -1; // cache.ElementCountInMemory; 
			}
		}

		/// <summary>
		/// Not ported yet
		/// </summary>
		public long ElementCountOnDisk
		{
			get
			{
				return -1; // cache.ElementCountOnDisk; 
			}
		}

		/// <summary>
		/// Not ported yet
		/// </summary>
		public long SizeInMemory
		{
			get
			{
				return -1; //  cache.SizeInMemory;
			}
		}

		/// <summary>
		/// Not ported yet
		/// </summary>
		public IDictionary Entries
		{
			get
			{
				IDictionary map = new Hashtable();
				//IDictionary<CacheKey, object> cacheMap = cache.ToMap();
				//foreach (KeyValuePair<CacheKey, object> me in cacheMap)
				//{
				//  map[me.Key.Key] = me.Value;
				//}
				return map;
			}
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder()
				.Append("SecondLevelCacheStatistics[")
				.Append("hitCount=").Append(hitCount)
				.Append(",missCount=").Append(missCount)
				.Append(",putCount=").Append(putCount);

			//not sure if this would ever be null but wanted to be careful
			if (cache != null)
			{
				buf.Append(",elementCountInMemory=").Append(ElementCountInMemory)
					.Append(",elementCountOnDisk=").Append(ElementCountOnDisk)
					.Append(",sizeInMemory=").Append(SizeInMemory);
			}
			buf.Append(']');
			return buf.ToString();
		}
	}
}
