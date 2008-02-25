using System;
using System.Collections;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using log4net;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Cache
{
	/// <summary>
	/// The standard implementation of the Hibernate <see cref="IQueryCache" />
	/// interface.  This implementation is very good at recognizing stale query
	/// results and re-running queries when it detects this condition, recaching
	/// the new results.
	/// </summary>
	public class StandardQueryCache : IQueryCache
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(StandardQueryCache));

		private readonly ICache queryCache;
		private readonly UpdateTimestampsCache updateTimestampsCache;
		private readonly string regionName;

		public string RegionName
		{
			get { return regionName; }
		}

		public void Clear()
		{
			queryCache.Clear();
		}

		public StandardQueryCache(Settings settings, IDictionary<string,string> props, UpdateTimestampsCache updateTimestampsCache,
		                          string regionName)
		{
			if (regionName == null)
			{
				regionName = typeof(StandardQueryCache).FullName;
			}
			String prefix = settings.CacheRegionPrefix;
			if (prefix != null) regionName = prefix + '.' + regionName;

			log.Info("starting query cache at region: " + regionName);
			queryCache = settings.CacheProvider.BuildCache(regionName, props);
			this.updateTimestampsCache = updateTimestampsCache;
			this.regionName = regionName;
		}

		public bool Put(QueryKey key, ICacheAssembler[] returnTypes, IList result, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("caching query results in region: " + regionName);
			}
			IList cacheable = new ArrayList(result.Count + 1);
			cacheable.Add(session.Timestamp);
			for (int i = 0; i < result.Count; i++)
			{
				if (returnTypes.Length == 1)
				{
					cacheable.Add(returnTypes[0].Disassemble(result[i], session, null));
				}
				else
				{
					cacheable.Add(TypeFactory.Disassemble((object[])result[i], returnTypes, null, session, null));
				}
			}
			queryCache.Put(key, cacheable);
			return true;
		}

		public IList Get(QueryKey key, ICacheAssembler[] returnTypes, ISet<string> spaces, ISessionImplementor session)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("checking cached query results in region: " + regionName);
			}
			IList cacheable = (IList) queryCache.Get(key);
			if (cacheable == null)
			{
				log.Debug("query results were not found in cache");
				return null;
			}
			IList result = new ArrayList(cacheable.Count - 1);
			long timestamp = (long) cacheable[0];
			log.Debug("Checking query spaces for up-to-dateness [" + spaces + "]");
			if (! IsUpToDate(spaces, timestamp))
			{
				log.Debug("cached query results were not up to date");
				return null;
			}
			log.Debug("returning cached query results");
			for (int i = 1; i < cacheable.Count; i++)
			{
				if (returnTypes.Length == 1)
				{
					result.Add(returnTypes[0].Assemble(cacheable[i], session, null));
				}
				else
				{
					result.Add(TypeFactory.Assemble((object[]) cacheable[i], returnTypes, session, null));
				}
			}
			return result;
		}

		protected bool IsUpToDate(ISet<string> spaces, long timestamp)
		{
			return updateTimestampsCache.IsUpToDate(spaces, timestamp);
		}

		public void Destroy()
		{
			try
			{
				queryCache.Destroy();
			}
			catch (Exception e)
			{
				log.Warn("could not destroy query cache: " + regionName, e);
			}
		}
	}
}