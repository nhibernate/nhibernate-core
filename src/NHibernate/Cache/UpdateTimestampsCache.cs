using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using NHibernate.Cfg;

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
		private ICache updateTimestamps;
		private readonly IBatchableReadOnlyCache _batchUpdateTimestamps;

		private readonly string regionName = typeof(UpdateTimestampsCache).Name;

		public virtual void Clear()
		{
			updateTimestamps.Clear();
		}

		public UpdateTimestampsCache(Settings settings, IDictionary<string, string> props)
		{
			string prefix = settings.CacheRegionPrefix;
			regionName = prefix == null ? regionName : prefix + '.' + regionName;
			log.Info("starting update timestamps cache at region: {0}", regionName);
			updateTimestamps = settings.CacheProvider.BuildCache(regionName, props);
			// ReSharper disable once SuspiciousTypeConversion.Global
			_batchUpdateTimestamps = updateTimestamps as IBatchableReadOnlyCache;
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
			long ts = updateTimestamps.NextTimestamp() + updateTimestamps.Timeout;
			foreach (var space in spaces)
			{
				updateTimestamps.Put(space, ts);
			}

			//TODO: return new Lock(ts);
		}

		//Since v5.1
		[Obsolete("Please use PreInvalidate(IReadOnlyCollection<string>) instead.")]
		public void Invalidate(object[] spaces)
		{
			//Only for backwards compatibility.
			Invalidate(spaces.OfType<string>().ToList());
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public virtual void Invalidate(IReadOnlyCollection<string> spaces)
		{
			//TODO: to handle concurrent writes correctly, the client should pass in a Lock
			long ts = updateTimestamps.NextTimestamp();
			//TODO: if lock.getTimestamp().equals(ts)
			foreach (var space in spaces)
			{
				log.Debug("Invalidating space [{0}]", space);
				updateTimestamps.Put(space, ts);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public virtual bool IsUpToDate(ISet<string> spaces, long timestamp /* H2.1 has Long here */)
		{
			if (_batchUpdateTimestamps != null)
			{
				var keys = new object[spaces.Count];
				var index = 0;
				foreach (var space in spaces)
				{
					keys[index++] = space;
				}
				var lastUpdates = _batchUpdateTimestamps.GetMany(keys);
				foreach (var lastUpdate in lastUpdates)
				{
					if (IsOutdated(lastUpdate, timestamp))
					{
						return false;
					}
				}
				return true;
			}

			foreach (string space in spaces)
			{
				object lastUpdate = updateTimestamps.Get(space);
				if (IsOutdated(lastUpdate, timestamp))
				{
					return false;
				}
				
			}
			return true;
		}

		public virtual void Destroy()
		{
			try
			{
				updateTimestamps.Destroy();
			}
			catch (Exception e)
			{
				log.Warn(e, "could not destroy UpdateTimestamps cache");
			}
		}

		private bool IsOutdated(object lastUpdate, long timestamp)
		{
			if (lastUpdate == null)
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
				if ((long) lastUpdate >= timestamp)
				{
					return true;
				}
			}

			return false;
		}
	}
}
