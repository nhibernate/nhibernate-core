using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using NHibernate.Cfg;
using NHibernate.Util;

namespace NHibernate.Cache
{
	/// <summary>
	/// Tracks the timestamps of the most recent updates to particular tables. It is
	/// important that the cache timeout of the underlying cache implementation be set
	/// to a higher value than the timeouts of any of the query caches. In fact, we 
	/// recommend that the the underlying cache not be configured for expiry at all.
	/// Note, in particular, that an LRU cache expiry policy is never appropriate.
	/// </summary>
	public partial class UpdateTimestampsCache
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(UpdateTimestampsCache));
		private readonly CacheBase _updateTimestamps;

		public virtual void Clear()
		{
			_updateTimestamps.Clear();
		}

		// Since v5.3
		[Obsolete("Please use overload with a CacheBase parameter.")]
		public UpdateTimestampsCache(Settings settings, IDictionary<string, string> props)
			: this(
				CacheFactory.BuildCacheBase(
					settings.GetFullCacheRegionName(nameof(UpdateTimestampsCache)),
					settings,
					props))
		{
		}

		/// <summary>
		/// Build the update timestamps cache.
		/// </summary>x
		/// <param name="cache">The <see cref="ICache" /> to use.</param>
		public UpdateTimestampsCache(CacheBase cache)
		{
			log.Info("starting update timestamps cache at region: {0}", cache.RegionName);
			_updateTimestamps = cache;
		}

		//Since v5.1
		[Obsolete("Please use PreInvalidate(IReadOnlyCollection<string>) instead.")]
		public void PreInvalidate(object[] spaces)
		{
			//Only for backwards compatibility.
			PreInvalidate(spaces.OfType<string>().ToList());
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public virtual void PreInvalidate(IReadOnlyCollection<string> spaces)
		{
			//TODO: to handle concurrent writes correctly, this should return a Lock to the client
			var ts = _updateTimestamps.NextTimestamp() + _updateTimestamps.Timeout;
			SetSpacesTimestamp(spaces, ts);

			//TODO: return new Lock(ts);
		}

		//Since v5.1
		[Obsolete("Please use Invalidate(IReadOnlyCollection<string>) instead.")]
		public void Invalidate(object[] spaces)
		{
			//Only for backwards compatibility.
			Invalidate(spaces.OfType<string>().ToList());
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public virtual void Invalidate(IReadOnlyCollection<string> spaces)
		{
			//TODO: to handle concurrent writes correctly, the client should pass in a Lock
			long ts = _updateTimestamps.NextTimestamp();
			//TODO: if lock.getTimestamp().equals(ts)
			if (log.IsDebugEnabled())
				log.Debug("Invalidating spaces [{0}]", StringHelper.CollectionToString(spaces));
			SetSpacesTimestamp(spaces, ts);
		}

		private void SetSpacesTimestamp(IReadOnlyCollection<string> spaces, long ts)
		{
			if (spaces.Count == 0)
				return;

			_updateTimestamps.PutMany(
				spaces.ToArray<object>(),
				ArrayHelper.Fill<object>(ts, spaces.Count));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public virtual bool IsUpToDate(ISet<string> spaces, long timestamp /* H2.1 has Long here */)
		{
			if (spaces.Count == 0)
				return true;

			var lastUpdates = _updateTimestamps.GetMany(spaces.ToArray<object>());
			return lastUpdates.All(lastUpdate => !IsOutdated(lastUpdate as long?, timestamp));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public virtual bool[] AreUpToDate(ISet<string>[] spaces, long[] timestamps)
		{
			if (spaces.Length == 0)
				return Array.Empty<bool>();

			var allSpaces = new HashSet<string>();
			foreach (var sp in spaces)
			{
				allSpaces.UnionWith(sp);
			}

			if (allSpaces.Count == 0)
				return ArrayHelper.Fill(true, spaces.Length);

			var keys = allSpaces.ToArray<object>();

			var index = 0;
			var lastUpdatesBySpace =
				_updateTimestamps
					.GetMany(keys)
					.ToDictionary(u => keys[index++], u => u as long?);

			var results = new bool[spaces.Length];
			for (var i = 0; i < spaces.Length; i++)
			{
				var timestamp = timestamps[i];
				results[i] = spaces[i].All(space => !IsOutdated(lastUpdatesBySpace[space], timestamp));
			}

			return results;
		}

		// Since v5.3
		[Obsolete("This method has no usages anymore")]
		public virtual void Destroy()
		{
			// The cache is externally provided and may be shared. Destroying the cache is
			// not the responsibility of this class.
		}

		private static bool IsOutdated(long? lastUpdate, long timestamp)
		{
			if (!lastUpdate.HasValue)
			{
				//the last update timestamp was lost from the cache
				//(or there were no updates since startup!)

				//NOTE: commented out, since users found the "safe" behavior
				//      counter-intuitive when testing, and we couldn't deal
				//      with all the forum posts :-(
				//updateTimestamps.put( space, new Long( updateTimestamps.nextTimestamp() ) );
				//result = false; // safer

				//OR: put a timestamp there, to avoid subsequent expensive
				//    lookups to a distributed cache - this is no good, since
				//    it is non-threadsafe (could hammer effect of an actual
				//    invalidation), and because this is not the way our
				//    preferred distributed caches work (they work by
				//    replication)
				//updateTimestamps.put( space, new Long(Long.MIN_VALUE) );
			}
			else
			{
				if (lastUpdate >= timestamp)
				{
					return true;
				}
			}

			return false;
		}
	}
}
