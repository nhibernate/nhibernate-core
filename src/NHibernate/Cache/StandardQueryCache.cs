using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public partial class StandardQueryCache : IQueryCache, IBatchableQueryCache
	{
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof (StandardQueryCache));
		private readonly string _regionName;
		private readonly UpdateTimestampsCache _updateTimestampsCache;
		// 6.0 TODO: remove
		private readonly CacheBase _cache;

		// Since v5.2
		[Obsolete("Please use overload with an ICache parameter.")]
		public StandardQueryCache(
			Settings settings,
			IDictionary<string, string> props,
			UpdateTimestampsCache updateTimestampsCache,
			string regionName)
			: this(
				updateTimestampsCache,
				settings.CacheProvider.BuildCache(
					(!string.IsNullOrEmpty(settings.CacheRegionPrefix) ? settings.CacheRegionPrefix + '.' : "") +
					(regionName ?? typeof (StandardQueryCache).FullName),
					props))
		{
		}

		// Since v5.2
		[Obsolete]
		private StandardQueryCache(
			UpdateTimestampsCache updateTimestampsCache,
			ICache cache)
			: this(updateTimestampsCache, cache as CacheBase ?? new ObsoleteCacheWrapper(cache))
		{
		}

		/// <summary>
		/// Build a query cache.
		/// </summary>
		/// <param name="updateTimestampsCache">The cache of updates timestamps.</param>
		/// <param name="regionCache">The <see cref="ICache" /> to use for the region.</param>
		public StandardQueryCache(
			UpdateTimestampsCache updateTimestampsCache,
			CacheBase regionCache)
		{
			if (regionCache == null)
				throw new ArgumentNullException(nameof(regionCache));

			_regionName = regionCache.RegionName;
			Log.Info("starting query cache at region: {0}", _regionName);

			Cache = regionCache;
			_cache = regionCache;
			_updateTimestampsCache = updateTimestampsCache;
		}

		#region IQueryCache Members

		// 6.0 TODO: type as CacheBase instead
#pragma warning disable 618
		public ICache Cache { get; }
#pragma warning restore 618

		public string RegionName
		{
			get { return _regionName; }
		}

		public void Clear()
		{
			Cache.Clear();
		}

		/// <inheritdoc />
		public bool Put(
			QueryKey key,
			QueryParameters queryParameters,
			ICacheAssembler[] returnTypes,
			IList result,
			ISessionImplementor session)
		{
			// 6.0 TODO: inline the call.
#pragma warning disable 612
			return Put(key, returnTypes, result, queryParameters.NaturalKeyLookup, session);
#pragma warning restore 612
		}

		// Since 5.2
		[Obsolete]
		public bool Put(QueryKey key, ICacheAssembler[] returnTypes, IList result, bool isNaturalKeyLookup, ISessionImplementor session)
		{
			if (isNaturalKeyLookup && result.Count == 0)
				return false;

			var ts = session.Factory.Settings.CacheProvider.NextTimestamp();

			Log.Debug("caching query results in region: '{0}'; {1}", _regionName, key);

			Cache.Put(key, GetCacheableResult(returnTypes, session, result, ts));

			return true;
		}

		/// <inheritdoc />
		public IList Get(
			QueryKey key,
			QueryParameters queryParameters,
			ICacheAssembler[] returnTypes,
			ISet<string> spaces,
			ISessionImplementor session)
		{
			var persistenceContext = session.PersistenceContext;
			var defaultReadOnlyOrig = persistenceContext.DefaultReadOnly;

			if (queryParameters.IsReadOnlyInitialized)
				persistenceContext.DefaultReadOnly = queryParameters.ReadOnly;
			else
				queryParameters.ReadOnly = persistenceContext.DefaultReadOnly;

			try
			{
				// 6.0 TODO: inline the call.
#pragma warning disable 612
				return Get(key, returnTypes, queryParameters.NaturalKeyLookup, spaces, session);
#pragma warning restore 612
			}
			finally
			{
				persistenceContext.DefaultReadOnly = defaultReadOnlyOrig;
			}
		}

		// Since 5.2
		[Obsolete]
		public IList Get(QueryKey key, ICacheAssembler[] returnTypes, bool isNaturalKeyLookup, ISet<string> spaces, ISessionImplementor session)
		{
			if (Log.IsDebugEnabled())
				Log.Debug("checking cached query results in region: '{0}'; {1}", _regionName, key);

			var cacheable = (IList) Cache.Get(key);
			if (cacheable == null)
			{
				Log.Debug("query results were not found in cache: {0}", key);
				return null;
			}

			var timestamp = (long) cacheable[0];

			if (Log.IsDebugEnabled())
				Log.Debug("Checking query spaces for up-to-dateness [{0}]", StringHelper.CollectionToString(spaces));

			if (!isNaturalKeyLookup && !IsUpToDate(spaces, timestamp))
			{
				Log.Debug("cached query results were not up to date for: {0}", key);
				return null;
			}

			return GetResultFromCacheable(key, returnTypes, isNaturalKeyLookup, session, cacheable);
		}

		/// <inheritdoc />
		public bool[] PutMany(
			QueryKey[] keys,
			QueryParameters[] queryParameters,
			ICacheAssembler[][] returnTypes,
			IList[] results,
			ISessionImplementor session)
		{
			if (Log.IsDebugEnabled())
				Log.Debug("caching query results in region: '{0}'; {1}", _regionName, StringHelper.CollectionToString(keys));

			var cached = new bool[keys.Length];
			var ts = session.Factory.Settings.CacheProvider.NextTimestamp();
			var cachedKeys = new List<object>();
			var cachedResults = new List<object>();
			for (var i = 0; i < keys.Length; i++)
			{
				var result = results[i];
				if (queryParameters[i].NaturalKeyLookup && result.Count == 0)
					continue;

				cached[i] = true;
				cachedKeys.Add(keys[i]);
				cachedResults.Add(GetCacheableResult(returnTypes[i], session, result, ts));
			}

			_cache.PutMany(cachedKeys.ToArray(), cachedResults.ToArray());

			return cached;
		}

		/// <inheritdoc />
		public IList[] GetMany(
			QueryKey[] keys,
			QueryParameters[] queryParameters,
			ICacheAssembler[][] returnTypes,
			ISet<string>[] spaces,
			ISessionImplementor session)
		{
			if (Log.IsDebugEnabled())
				Log.Debug("checking cached query results in region: '{0}'; {1}", _regionName, StringHelper.CollectionToString(keys));

			var cacheables = _cache.GetMany(keys).Cast<IList>().ToArray();

			var spacesToCheck = new List<ISet<string>>();
			var checkedSpacesIndexes = new HashSet<int>();
			var checkedSpacesTimestamp = new List<long>();
			for (var i = 0; i < keys.Length; i++)
			{
				var cacheable = cacheables[i];
				if (cacheable == null)
				{
					Log.Debug("query results were not found in cache: {0}", keys[i]);
					continue;
				}

				var querySpaces = spaces[i];
				if (queryParameters[i].NaturalKeyLookup || querySpaces.Count == 0)
					continue;

				spacesToCheck.Add(querySpaces);
				checkedSpacesIndexes.Add(i);
				// The timestamp is the first element of the cache result.
				checkedSpacesTimestamp.Add((long) cacheable[0]);
				if (Log.IsDebugEnabled())
					Log.Debug("Checking query spaces for up-to-dateness [{0}]", StringHelper.CollectionToString(querySpaces));
			}

			var upToDates = spacesToCheck.Count > 0
				? _updateTimestampsCache.AreUpToDate(spacesToCheck.ToArray(), checkedSpacesTimestamp.ToArray())
				: Array.Empty<bool>();

			var upToDatesIndex = 0;
			var persistenceContext = session.PersistenceContext;
			var defaultReadOnlyOrig = persistenceContext.DefaultReadOnly;
			var results = new IList[keys.Length];
			for (var i = 0; i < keys.Length; i++)
			{
				var cacheable = cacheables[i];
				if (cacheable == null)
					continue;

				var key = keys[i];
				if (checkedSpacesIndexes.Contains(i) && !upToDates[upToDatesIndex++])
				{
					Log.Debug("cached query results were not up to date for: {0}", key);
					continue;
				}

				var queryParams = queryParameters[i];
				if (queryParams.IsReadOnlyInitialized)
					persistenceContext.DefaultReadOnly = queryParams.ReadOnly;
				else
					queryParams.ReadOnly = persistenceContext.DefaultReadOnly;

				// Adjust the session cache mode, as GetResultFromCacheable assemble types which may cause
				// entity loads, which may interact with the cache.
				using (session.SwitchCacheMode(queryParams.CacheMode))
				{
					try
					{
						results[i] = GetResultFromCacheable(
							key,
							returnTypes[i],
							queryParams.NaturalKeyLookup,
							session,
							cacheable);
					}
					finally
					{
						persistenceContext.DefaultReadOnly = defaultReadOnlyOrig;
					}
				}
			}

			return results;
		}

		public void Destroy()
		{
			try
			{
				Cache.Destroy();
			}
			catch (Exception e)
			{
				Log.Warn(e, "could not destroy query cache: {0}", _regionName);
			}
		}

		#endregion

		private static List<object> GetCacheableResult(
			ICacheAssembler[] returnTypes,
			ISessionImplementor session,
			IList result,
			long ts)
		{
			var cacheable = new List<object>(result.Count + 1) { ts };
			foreach (var row in result)
			{
				if (returnTypes.Length == 1)
				{
					cacheable.Add(returnTypes[0].Disassemble(row, session, null));
				}
				else
				{
					cacheable.Add(TypeHelper.Disassemble((object[])row, returnTypes, null, session, null));
				}
			}

			return cacheable;
		}

		private IList GetResultFromCacheable(
			QueryKey key,
			ICacheAssembler[] returnTypes,
			bool isNaturalKeyLookup,
			ISessionImplementor session,
			IList cacheable)
		{
			Log.Debug("returning cached query results for: {0}", key);
			if (key.ResultTransformer?.AutoDiscoverTypes == true && cacheable.Count > 0)
			{
				returnTypes = GuessTypes(cacheable);
			}

			try
			{
				var result = new List<object>(cacheable.Count - 1);
				if (returnTypes.Length == 1)
				{
					var returnType = returnTypes[0];

					// Skip first element, it is the timestamp
					var rows = new List<object>(cacheable.Count - 1);
					for (var i = 1; i < cacheable.Count; i++)
					{
						rows.Add(cacheable[i]);
					}

					foreach (var row in rows)
					{
						returnType.BeforeAssemble(row, session);
					}

					foreach (var row in rows)
					{
						result.Add(returnType.Assemble(row, session, null));
					}
				}
				else
				{
					// Skip first element, it is the timestamp
					var rows = new List<object[]>(cacheable.Count - 1);
					for (var i = 1; i < cacheable.Count; i++)
					{
						rows.Add((object[]) cacheable[i]);
					}

					foreach (var row in rows)
					{
						TypeHelper.BeforeAssemble(row, returnTypes, session);
					}

					foreach (var row in rows)
					{
						result.Add(TypeHelper.Assemble(row, returnTypes, session, null));
					}
				}

				return result;
			}
			catch (UnresolvableObjectException ex)
			{
				if (isNaturalKeyLookup)
				{
					//TODO: not really completely correct, since
					//      the UnresolvableObjectException could occur while resolving
					//      associations, leaving the PC in an inconsistent state
					Log.Debug(ex, "could not reassemble cached result set");
					// Handling a RemoveMany here does not look worth it, as this case short-circuits
					// the result-set. So a Many could only benefit batched queries, and only if many
					// of them are natural key lookup with an unresolvable object case.
					Cache.Remove(key);
					return null;
				}

				throw;
			}
		}

		private static ICacheAssembler[] GuessTypes(IList cacheable)
		{
			var colCount = (cacheable[0] as object[])?.Length ?? 1;
			var returnTypes = new ICacheAssembler[colCount];
			if (colCount == 1)
			{
				foreach (var obj in cacheable)
				{
					if (obj == null)
						continue;
					returnTypes[0] = NHibernateUtil.GuessType(obj);
					break;
				}
			}
			else
			{
				var foundTypes = 0;
				foreach (object[] row in cacheable)
				{
					for (var i = 0; i < colCount; i++)
					{
						if (row[i] != null && returnTypes[i] == null)
						{
							returnTypes[i] = NHibernateUtil.GuessType(row[i]);
							foundTypes++;
						}
					}
					if (foundTypes == colCount)
						break;
				}
			}
			// If a column value was null for all rows, its type is still null: put a type which will just yield null
			// on null value.
			for (var i = 0; i < colCount; i++)
			{
				if (returnTypes[i] == null)
					returnTypes[i] = NHibernateUtil.String;
			}
			return returnTypes;
		}

		protected virtual bool IsUpToDate(ISet<string> spaces, long timestamp)
		{
			return _updateTimestampsCache.IsUpToDate(spaces, timestamp);
		}
	}
}
