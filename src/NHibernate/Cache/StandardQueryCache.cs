using System;
using System.Collections;
using System.Collections.Generic;

using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Cache
{
	/// <summary>
	/// The standard implementation of the Hibernate <see cref="IQueryCache" />
	/// interface.  This implementation is very good at recognizing stale query
	/// results and re-running queries when it detects this condition, recaching
	/// the new results.
	/// </summary>
	public partial class StandardQueryCache : IQueryCache
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof (StandardQueryCache));
		private readonly ICache _queryCache;
		private readonly string _regionName;
		private readonly UpdateTimestampsCache _updateTimestampsCache;

		public StandardQueryCache(Settings settings, IDictionary<string, string> props, UpdateTimestampsCache updateTimestampsCache, string regionName)
		{
			if (regionName == null)
				regionName = typeof (StandardQueryCache).FullName;

			String prefix = settings.CacheRegionPrefix;
			if (!string.IsNullOrEmpty(prefix))
				regionName = prefix + '.' + regionName;

			Log.Info("starting query cache at region: " + regionName);

			_queryCache = settings.CacheProvider.BuildCache(regionName, props);
			_updateTimestampsCache = updateTimestampsCache;
			_regionName = regionName;
		}

		#region IQueryCache Members

		public ICache Cache
		{
			get { return _queryCache; }
		}

		public string RegionName
		{
			get { return _regionName; }
		}

		public void Clear()
		{
			_queryCache.Clear();
		}

		public bool Put(QueryKey key, ICacheAssembler[] returnTypes, IList result, bool isNaturalKeyLookup, ISessionImplementor session)
		{
			if (isNaturalKeyLookup && result.Count == 0)
				return false;

			long ts = session.Timestamp;

			if (Log.IsDebugEnabled)
				Log.DebugFormat("caching query results in region: '{0}'; {1}", _regionName, key);

			IList cacheable = new List<object>(result.Count + 1) {ts};
			for (int i = 0; i < result.Count; i++)
			{
				if (returnTypes.Length == 1)
				{
					cacheable.Add(returnTypes[0].Disassemble(result[i], session, null));
				}
				else
				{
					cacheable.Add(TypeHelper.Disassemble((object[]) result[i], returnTypes, null, session, null));
				}
			}

			_queryCache.Put(key, cacheable);

			return true;
		}

		public IList Get(QueryKey key, ICacheAssembler[] returnTypes, bool isNaturalKeyLookup, ISet<string> spaces, ISessionImplementor session)
		{
			if (Log.IsDebugEnabled)
				Log.DebugFormat("checking cached query results in region: '{0}'; {1}", _regionName, key);

			var cacheable = (IList)_queryCache.Get(key);
			if (cacheable == null)
			{
				Log.DebugFormat("query results were not found in cache: {0}", key);
				return null;
			}

			var timestamp = (long)cacheable[0];

			if (Log.IsDebugEnabled)
				Log.DebugFormat("Checking query spaces for up-to-dateness [{0}]", StringHelper.CollectionToString(spaces));

			if (!isNaturalKeyLookup && !IsUpToDate(spaces, timestamp))
			{
				Log.DebugFormat("cached query results were not up to date for: {0}", key);
				return null;
			}

			Log.DebugFormat("returning cached query results for: {0}", key);
			for (int i = 1; i < cacheable.Count; i++)
			{
				if (returnTypes.Length == 1)
				{
					returnTypes[0].BeforeAssemble(cacheable[i], session);
				}
				else
				{
					TypeHelper.BeforeAssemble((object[])cacheable[i], returnTypes, session);
				}
			}

			IList result = new List<object>(cacheable.Count - 1);
			for (int i = 1; i < cacheable.Count; i++)
			{
				try
				{
					if (returnTypes.Length == 1)
					{
						result.Add(returnTypes[0].Assemble(cacheable[i], session, null));
					}
					else
					{
						result.Add(TypeHelper.Assemble((object[])cacheable[i], returnTypes, session, null));
					}
				}
				catch (UnresolvableObjectException)
				{
					if (isNaturalKeyLookup)
					{
						//TODO: not really completely correct, since
						//      the UnresolvableObjectException could occur while resolving
						//      associations, leaving the PC in an inconsistent state
						Log.Debug("could not reassemble cached result set");
						_queryCache.Remove(key);
						return null;
					}

					throw;
				}
			}

			return result;
		}

		public void Destroy()
		{
			try
			{
				_queryCache.Destroy();
			}
			catch (Exception e)
			{
				Log.Warn("could not destroy query cache: " + _regionName, e);
			}
		}

		#endregion

		protected virtual bool IsUpToDate(ISet<string> spaces, long timestamp)
		{
			return _updateTimestampsCache.IsUpToDate(spaces, timestamp);
		}
	}
}