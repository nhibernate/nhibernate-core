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
		private readonly ICache cache;
		internal long hitCount;
		internal long missCount;
		internal long putCount;

		public SecondLevelCacheStatistics(ICache cache) : base(cache.RegionName)
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
