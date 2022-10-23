using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
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
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(StandardQueryCache));
		private readonly string _regionName;
		private readonly UpdateTimestampsCache _updateTimestampsCache;
		private readonly CacheBase _cache;

		// Since v5.3
		[Obsolete("Please use overload with a CacheBase parameter.")]
		public StandardQueryCache(
			Settings settings,
			IDictionary<string, string> props,
			UpdateTimestampsCache updateTimestampsCache,
			string regionName)
			: this(
				updateTimestampsCache,
				CacheFactory.BuildCacheBase(
					settings.GetFullCacheRegionName(regionName ?? typeof(StandardQueryCache).FullName),
					settings,
					props))
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

			_cache = regionCache;
			_updateTimestampsCache = updateTimestampsCache;
		}

		#region IQueryCache Members

		// 6.0 TODO: type as CacheBase instead
#pragma warning disable 618
		public ICache Cache => _cache;
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

			Cache.Put(key, GetCacheableResult(returnTypes, session, result, ts, GetAutoDiscoveredAliases(key)));

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
				if (Log.IsDebugEnabled())
					Log.Debug("checking cached query results in region: '{0}'; {1}", _regionName, key);

				var cacheable = (IList) Cache.Get(key);
				if (cacheable == null)
				{
					Log.Debug("query results were not found in cache: {0}", key);
					return null;
				}

				var timestamp = GetResultsMetadata(cacheable, out var aliases);

				if (Log.IsDebugEnabled())
					Log.Debug("Checking query spaces for up-to-dateness [{0}]", StringHelper.CollectionToString(spaces));

				if (!queryParameters.NaturalKeyLookup && !IsUpToDate(spaces, timestamp))
				{
					Log.Debug("cached query results were not up to date for: {0}", key);
					return null;
				}

				var result = GetResultFromCacheable(key, returnTypes, queryParameters.NaturalKeyLookup, session, cacheable);

				if (result != null && key.ResultTransformer?.AutoDiscoverTypes == true && result.Count > 0)
				{
					key.ResultTransformer.SupplyAutoDiscoveredParameters(queryParameters.ResultTransformer, aliases);
				}

				return result;
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

			var timestamp = GetResultsMetadata(cacheable, out var _);

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

				var key = keys[i];
				cached[i] = true;
				cachedKeys.Add(key);
				cachedResults.Add(GetCacheableResult(returnTypes[i], session, result, ts, GetAutoDiscoveredAliases(key)));
			}

			_cache.PutMany(cachedKeys.ToArray(), cachedResults.ToArray());

			return cached;
		}

		private static string[] GetAutoDiscoveredAliases(QueryKey key)
		{
			return key.ResultTransformer?.AutoDiscoveredAliases;
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

			var cacheables = _cache.GetMany(keys);

			var spacesToCheck = new List<ISet<string>>();
			var checkedSpacesIndexes = new HashSet<int>();
			var checkedSpacesTimestamp = new List<long>();
			for (var i = 0; i < keys.Length; i++)
			{
				var cacheable = (IList) cacheables[i];
				if (cacheable == null)
				{
					Log.Debug("query results were not found in cache: {0}", keys[i]);
					continue;
				}

				var timestamp = GetResultsMetadata(cacheable, out var aliases); var key = keys[i];
				if (key.ResultTransformer?.AutoDiscoverTypes == true && HasResults(cacheable))
				{
					key.ResultTransformer.SupplyAutoDiscoveredParameters(queryParameters[i].ResultTransformer, aliases);
				}

				var querySpaces = spaces[i];
				if (queryParameters[i].NaturalKeyLookup || querySpaces.Count == 0)
					continue;

				spacesToCheck.Add(querySpaces);
				checkedSpacesIndexes.Add(i);
				checkedSpacesTimestamp.Add(timestamp);
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
			var finalReturnTypes = new ICacheAssembler[keys.Length][];
			try
			{
				session.PersistenceContext.BatchFetchQueue.InitializeQueryCacheQueue();

				for (var i = 0; i < keys.Length; i++)
				{
					var cacheable = (IList) cacheables[i];
					if (cacheable == null)
						continue;

					var key = keys[i];
					if (checkedSpacesIndexes.Contains(i) && !upToDates[upToDatesIndex++])
					{
						Log.Debug("cached query results were not up to date for: {0}", key);
						cacheables[i] = null;
						continue;
					}

					var queryParams = queryParameters[i];
					if (queryParams.IsReadOnlyInitialized)
						persistenceContext.DefaultReadOnly = queryParams.ReadOnly;
					else
						queryParams.ReadOnly = persistenceContext.DefaultReadOnly;

					Log.Debug("returning cached query results for: {0}", key);

					finalReturnTypes[i] = GetReturnTypes(key, returnTypes[i], cacheable);
					PerformBeforeAssemble(finalReturnTypes[i], session, cacheable);
				}

				for (var i = 0; i < keys.Length; i++)
				{
					if (cacheables[i] == null)
					{
						continue;
					}

					var queryParams = queryParameters[i];
					// Adjust the session cache mode, as PerformAssemble assemble types which may cause
					// entity loads, which may interact with the cache.
					using (session.SwitchCacheMode(queryParams.CacheMode))
					{
						try
						{
							results[i] = PerformAssemble(keys[i], finalReturnTypes[i], queryParams.NaturalKeyLookup, session, (IList) cacheables[i]);
						}
						finally
						{
							persistenceContext.DefaultReadOnly = defaultReadOnlyOrig;
						}
					}
				}

				for (var i = 0; i < keys.Length; i++)
				{
					if (cacheables[i] == null)
					{
						continue;
					}

					var queryParams = queryParameters[i];
					// Adjust the session cache mode, as InitializeCollections will initialize collections,
					// which may interact with the cache.
					using (session.SwitchCacheMode(queryParams.CacheMode))
					{
						try
						{
							InitializeCollections(finalReturnTypes[i], session, results[i], (IList) cacheables[i]);
						}
						finally
						{
							persistenceContext.DefaultReadOnly = defaultReadOnlyOrig;
						}
					}
				}
			}
			finally
			{
				session.PersistenceContext.BatchFetchQueue.TerminateQueryCacheQueue();
			}

			return results;
		}

		public void Destroy()
		{
			// The cache is externally provided and may be shared. Destroying the cache is
			// not the responsibility of this class.
		}

		#endregion

		private static List<object> GetCacheableResult(
			ICacheAssembler[] returnTypes,
			ISessionImplementor session,
			IList result,
			long ts, string[] aliases)
		{
			var cacheable =
				new List<object>(result.Count + 1)
				{
					aliases == null ? ts : new object[] { ts, aliases.ToArray<object>() }
				};

			foreach (var row in result)
			{
				if (returnTypes.Length == 1)
				{
					cacheable.Add(returnTypes[0].Disassemble(row, session, null));
				}
				else
				{
					cacheable.Add(TypeHelper.Disassemble((object[]) row, returnTypes, null, session, null));
				}
			}

			return cacheable;
		}

		private static long GetResultsMetadata(IList cacheable, out string[] aliases)
		{
			aliases = null;

			var metadata = cacheable[0];
			var timestamp = metadata as long?;
			if (timestamp.HasValue)
				return timestamp.Value;

			var metadataArray = (object[]) metadata;
			aliases = ((object[]) metadataArray[1]).Cast<string>().ToArray();
			return (long) metadataArray[0];
		}

		private static bool HasResults(IList cacheable)
			// First element is the timestamp.
			=> cacheable.Count > 1;

		private static IEnumerable<object> GetResultsEnumerable(IList cacheable)
		{
			// Skip first element, it is the timestamp.
			for (var i = 1; i < cacheable.Count; i++)
			{
				yield return cacheable[i];
			}
		}

		private static ICacheAssembler[] GetReturnTypes(
			QueryKey key,
			ICacheAssembler[] returnTypes,
			IList cacheable)
		{
			var hasResults = HasResults(cacheable);
			if (key.ResultTransformer?.AutoDiscoverTypes == true && hasResults)
			{
				returnTypes = GuessTypes(cacheable);
			}

			if (hasResults && returnTypes == null)
			{
				throw new HibernateException("Return types for non empty query results are null, cannot assemble the results");
			}

			return returnTypes;
		}

		private static void PerformBeforeAssemble(
			ICacheAssembler[] returnTypes,
			ISessionImplementor session,
			IList cacheable)
		{
			if (!HasResults(cacheable))
				return;

			if (returnTypes.Length == 1)
			{
				var returnType = returnTypes[0];

				foreach (var cached in GetResultsEnumerable(cacheable))
				{
					returnType.BeforeAssemble(cached, session);
				}
			}
			else
			{
				foreach (var cached in GetResultsEnumerable(cacheable))
				{
					TypeHelper.BeforeAssemble((object[]) cached, returnTypes, session);
				}
			}
		}

		private IList PerformAssemble(
			QueryKey key,
			ICacheAssembler[] returnTypes,
			bool isNaturalKeyLookup,
			ISessionImplementor session,
			IList cacheable)
		{
			try
			{
				var result = new List<object>(cacheable.Count - 1);
				if (!HasResults(cacheable))
					return result;

				if (returnTypes.Length == 1)
				{
					var returnType = returnTypes[0];

					foreach (var cached in GetResultsEnumerable(cacheable))
					{
						result.Add(returnType.Assemble(cached, session, null));
					}
				}
				else
				{
					var nonCollectionTypeIndexes = new List<int>();
					for (var i = 0; i < returnTypes.Length; i++)
					{
						if (!(returnTypes[i] is CollectionType))
						{
							nonCollectionTypeIndexes.Add(i);
						}
					}

					foreach (var cached in GetResultsEnumerable(cacheable))
					{
						result.Add(TypeHelper.Assemble((object[]) cached, returnTypes, nonCollectionTypeIndexes, session));
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

		private static void InitializeCollections(
			ICacheAssembler[] returnTypes,
			ISessionImplementor session,
			IList assembleResult,
			IList cacheResult)
		{
			if (!HasResults(cacheResult))
				return;

			var collectionIndexes = new Dictionary<int, ICollectionPersister>();
			for (var i = 0; i < returnTypes.Length; i++)
			{
				if (returnTypes[i] is CollectionType collectionType)
				{
					collectionIndexes.Add(i, session.Factory.GetCollectionPersister(collectionType.Role));
				}
			}

			if (collectionIndexes.Count == 0)
			{
				return;
			}

			var j = 0;
			foreach (var cached in GetResultsEnumerable(cacheResult))
			{
				// Initialization of the fetched collection must be done at the end in order to be able to batch fetch them
				// from the cache or database. The collections were already created when their owners were assembled so we only
				// have to initialize them.
				TypeHelper.InitializeCollections(
					(object[]) cached,
					(object[]) assembleResult[j],
					collectionIndexes,
					session);
				j++;
			}
		}

		private IList GetResultFromCacheable(
			QueryKey key,
			ICacheAssembler[] returnTypes,
			bool isNaturalKeyLookup,
			ISessionImplementor session,
			IList cacheable)
		{
			Log.Debug("returning cached query results for: {0}", key);

			if (!HasResults(cacheable))
				return new List<object>();

			returnTypes = GetReturnTypes(key, returnTypes, cacheable);
			try
			{
				session.PersistenceContext.BatchFetchQueue.InitializeQueryCacheQueue();

				PerformBeforeAssemble(returnTypes, session, cacheable);
				var result = PerformAssemble(key, returnTypes, isNaturalKeyLookup, session, cacheable);
				InitializeCollections(returnTypes, session, result, cacheable);
				return result;
			}
			finally
			{
				session.PersistenceContext.BatchFetchQueue.TerminateQueryCacheQueue();
			}
		}

		private static ICacheAssembler[] GuessTypes(IList cacheable)
		{
			var colCount = (GetResultsEnumerable(cacheable).First() as object[])?.Length ?? 1;
			var returnTypes = new ICacheAssembler[colCount];
			if (colCount == 1)
			{
				foreach (var cached in GetResultsEnumerable(cacheable))
				{
					if (cached == null)
						continue;
					returnTypes[0] = NHibernateUtil.GuessType(cached);
					break;
				}
			}
			else
			{
				var foundTypes = 0;
				foreach (var cached in GetResultsEnumerable(cacheable))
				{
					var row = (object[]) cached;
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
			if (spaces.Count == 0)
				return true;

			return _updateTimestampsCache.IsUpToDate(spaces, timestamp);
		}
	}
}
