using System;

namespace NHibernate.Cache 
{
	/// <summary>
	/// Summary description for NonstrictReadWriteCache.
	/// </summary>
	public class NonstrictReadWriteCache : ICacheConcurrencyStrategy
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(NonstrictReadWriteCache));
		private static readonly long timeout = 10000;

		private readonly ICache cache;

		public NonstrictReadWriteCache(ICache cache) 
		{
			this.cache = cache;
		}

		#region ICacheConcurrencyStrategy Members

		public object Get(object key, long txTimestamp)
		{
			object result = cache.Get(key);
			if( result!=null & !(result is Int64) ) 
			{
				if (log.IsDebugEnabled) log.Debug("Cache hit: " + key);
				return result;
			}

			return null;
		}

		public bool Put(object key, object value, long txTimestamp)
		{
			object result = cache.Get(key);
			if(result==null) 
			{
				if (log.IsDebugEnabled) log.Debug("Caching new: " + key);
			}
			else if ( (result is Int64) && ( (Int64)result < txTimestamp / Timestamper.OneMs ) )
			{
				// note that this is not guaranteed to be correct in a cluster
				// because system times could be inconsistent
				if(log.IsDebugEnabled) log.Debug("Caching invalidated: " + key);
			}
			else 
			{
				return false; // note early exit
			}

			cache.Put(key, value);
			return true;
		}

		public void Lock(object key)
		{
			// in case the server crashes, we need the lock to timeout
			cache.Put( key, ( timeout + Timestamper.Next() / Timestamper.OneMs ) );
		}

		public void Release(object key)
		{
			if(log.IsDebugEnabled) log.Debug("Invalidating: " + key);

			//remove the lock (any later transactions can recache)
			cache.Put(key, Timestamper.Next() / Timestamper.OneMs);
		}

		public void Remove(object key)
		{
			if(log.IsDebugEnabled) log.Debug("Removing: " + key);
			cache.Remove(key);
		}

		public void Clear()
		{
			if(log.IsDebugEnabled) log.Debug("Clearing");
			cache.Clear();
		}

		public void Destroy()
		{
			try 
			{
				cache.Destroy();
			}
			catch(Exception e) 
			{
				log.Warn("Could not destroy cache", e);
			}
		}

		#endregion
	}
}
