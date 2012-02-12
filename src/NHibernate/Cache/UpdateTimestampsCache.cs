using System;
using System.Collections.Generic;
using System.Threading;
using Iesi.Collections.Generic;

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
    public class UpdateTimestampsCache
    {
        private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(UpdateTimestampsCache));
        private readonly ReaderWriterLockSlim rwl = new ReaderWriterLockSlim();
        private readonly ICache updateTimestamps;

        private readonly string regionName = typeof(UpdateTimestampsCache).Name;

        public void Clear()
        {
            updateTimestamps.Clear();
        }

        public UpdateTimestampsCache(Settings settings, IDictionary<string, string> props)
        {
            var prefix = settings.CacheRegionPrefix;
            regionName = prefix == null ? regionName : prefix + '.' + regionName;
            log.Info("starting update timestamps cache at region: " + regionName);
            updateTimestamps = settings.CacheProvider.BuildCache(regionName, props);
        }

        public void PreInvalidate(object[] spaces)
        {
            rwl.EnterWriteLock();
            try
            {
                //TODO: to handle concurrent writes correctly, this should return a Lock to the client
                var ts = updateTimestamps.NextTimestamp() + updateTimestamps.Timeout;
                for (var i = 0; i < spaces.Length; i++)
                {
                    updateTimestamps.Put(spaces[i], ts);
                }
                //TODO: return new Lock(ts);
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }

        public void Invalidate(object[] spaces)
        {
            rwl.EnterWriteLock();
            try
            {
                //TODO: to handle concurrent writes correctly, the client should pass in a Lock
                var ts = updateTimestamps.NextTimestamp();
                //TODO: if lock.getTimestamp().equals(ts)
                for (var i = 0; i < spaces.Length; i++)
                {
                    log.Debug(string.Format("Invalidating space [{0}]", spaces[i]));
                    updateTimestamps.Put(spaces[i], ts);
                }
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }

        public bool IsUpToDate(ISet<string> spaces, long timestamp /* H2.1 has Long here */)
        {
            rwl.EnterReadLock();
            try
            {
                foreach (var space in spaces)
                {
                    var lastUpdate = updateTimestamps.Get(space);
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
                        if ((long)lastUpdate >= timestamp)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            finally
            {
                rwl.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets timestamps of the given spaces.
        /// </summary>
        /// <param name="spaces"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method may be useful for getting a maximum timestamp of the given set of entities,
        /// for instance for the http ETag generation.
        /// </remarks>
        public long?[] GetTimestamps(string[] spaces)
        {
            rwl.EnterReadLock();
            try
            {
                var timestamps = new long?[spaces.Length];
                for (var i = 0; i < spaces.Length; i++)
                {
                    var lastUpdate = updateTimestamps.Get(spaces[i]);
                    if (lastUpdate == null)
                    {
                        timestamps[i] = null;
                    }
                    else
                    {
                        timestamps[i] = (long)lastUpdate;
                    }
                }
                return timestamps;
            }
            finally
            {
                rwl.ExitReadLock();
            }
        }

        public void Destroy()
        {
            try
            {
                updateTimestamps.Destroy();
                rwl.Dispose();
            }
            catch (Exception e)
            {
                log.Warn("could not destroy UpdateTimestamps cache", e);
            }
        }
    }
}