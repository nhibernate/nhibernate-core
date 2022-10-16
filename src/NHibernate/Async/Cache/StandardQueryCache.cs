﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


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
	using System.Threading.Tasks;
	using System.Threading;
	public partial class StandardQueryCache : IQueryCache, IBatchableQueryCache
	{

		#region IQueryCache Members

		public Task ClearAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return Cache.ClearAsync(cancellationToken);
		}

		/// <inheritdoc />
		public async Task<bool> PutAsync(
			QueryKey key,
			QueryParameters queryParameters,
			ICacheAssembler[] returnTypes,
			IList result,
			ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (queryParameters.NaturalKeyLookup && result.Count == 0)
				return false;

			var ts = session.Factory.Settings.CacheProvider.NextTimestamp();

			Log.Debug("caching query results in region: '{0}'; {1}", _regionName, key);

			await (Cache.PutAsync(key, await (GetCacheableResultAsync(returnTypes, session, result, ts, GetAutoDiscoveredAliases(key), cancellationToken)).ConfigureAwait(false), cancellationToken)).ConfigureAwait(false);

			return true;
		}

		// Since 5.2
		[Obsolete]
		public async Task<bool> PutAsync(QueryKey key, ICacheAssembler[] returnTypes, IList result, bool isNaturalKeyLookup, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (isNaturalKeyLookup && result.Count == 0)
				return false;

			var ts = session.Factory.Settings.CacheProvider.NextTimestamp();

			Log.Debug("caching query results in region: '{0}'; {1}", _regionName, key);

			await (Cache.PutAsync(key, await (GetCacheableResultAsync(returnTypes, session, result, ts, null, cancellationToken)).ConfigureAwait(false), cancellationToken)).ConfigureAwait(false);

			return true;
		}

		/// <inheritdoc />
		public async Task<IList> GetAsync(
			QueryKey key,
			QueryParameters queryParameters,
			ICacheAssembler[] returnTypes,
			ISet<string> spaces,
			ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
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

				var cacheable = (IList) await (Cache.GetAsync(key, cancellationToken)).ConfigureAwait(false);
				if (cacheable == null)
				{
					Log.Debug("query results were not found in cache: {0}", key);
					return null;
				}

				var timestamp = (long) cacheable[0];

				if (Log.IsDebugEnabled())
					Log.Debug("Checking query spaces for up-to-dateness [{0}]", StringHelper.CollectionToString(spaces));

				if (!queryParameters.NaturalKeyLookup && !await (IsUpToDateAsync(spaces, timestamp, cancellationToken)).ConfigureAwait(false))
				{
					Log.Debug("cached query results were not up to date for: {0}", key);
					return null;
				}

				var result = await (GetResultFromCacheableAsync(key, returnTypes, queryParameters.NaturalKeyLookup, session, cacheable, cancellationToken)).ConfigureAwait(false);

				if (result != null && key.ResultTransformer?.AutoDiscoverTypes == true && result.Count > 0)
				{
					var aliases = (string[]) cacheable[1];
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
		public async Task<IList> GetAsync(QueryKey key, ICacheAssembler[] returnTypes, bool isNaturalKeyLookup, ISet<string> spaces, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (Log.IsDebugEnabled())
				Log.Debug("checking cached query results in region: '{0}'; {1}", _regionName, key);

			var cacheable = (IList) await (Cache.GetAsync(key, cancellationToken)).ConfigureAwait(false);
			if (cacheable == null)
			{
				Log.Debug("query results were not found in cache: {0}", key);
				return null;
			}

			var timestamp = (long) cacheable[0];

			if (Log.IsDebugEnabled())
				Log.Debug("Checking query spaces for up-to-dateness [{0}]", StringHelper.CollectionToString(spaces));

			if (!isNaturalKeyLookup && !await (IsUpToDateAsync(spaces, timestamp, cancellationToken)).ConfigureAwait(false))
			{
				Log.Debug("cached query results were not up to date for: {0}", key);
				return null;
			}

			return await (GetResultFromCacheableAsync(key, returnTypes, isNaturalKeyLookup, session, cacheable, cancellationToken)).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<bool[]> PutManyAsync(
			QueryKey[] keys,
			QueryParameters[] queryParameters,
			ICacheAssembler[][] returnTypes,
			IList[] results,
			ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
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
				cachedResults.Add(await (GetCacheableResultAsync(returnTypes[i], session, result, ts, GetAutoDiscoveredAliases(key), cancellationToken)).ConfigureAwait(false));
			}

			await (_cache.PutManyAsync(cachedKeys.ToArray(), cachedResults.ToArray(), cancellationToken)).ConfigureAwait(false);

			return cached;
		}

		/// <inheritdoc />
		public async Task<IList[]> GetManyAsync(
			QueryKey[] keys,
			QueryParameters[] queryParameters,
			ICacheAssembler[][] returnTypes,
			ISet<string>[] spaces,
			ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (Log.IsDebugEnabled())
				Log.Debug("checking cached query results in region: '{0}'; {1}", _regionName, StringHelper.CollectionToString(keys));

			var cacheables = await (_cache.GetManyAsync(keys, cancellationToken)).ConfigureAwait(false);

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
				? await (_updateTimestampsCache.AreUpToDateAsync(spacesToCheck.ToArray(), checkedSpacesTimestamp.ToArray(), cancellationToken)).ConfigureAwait(false)
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
						continue;
					}

					var queryParams = queryParameters[i];
					if (queryParams.IsReadOnlyInitialized)
						persistenceContext.DefaultReadOnly = queryParams.ReadOnly;
					else
						queryParams.ReadOnly = persistenceContext.DefaultReadOnly;

					Log.Debug("returning cached query results for: {0}", key);

					finalReturnTypes[i] = GetReturnTypes(key, returnTypes[i], cacheable);
					await (PerformBeforeAssembleAsync(finalReturnTypes[i], session, cacheable, cancellationToken)).ConfigureAwait(false);

					if (key.ResultTransformer?.AutoDiscoverTypes == true && cacheable.Count > 2)
					{
						var aliases = (string[]) cacheable[1];
						key.ResultTransformer.SupplyAutoDiscoveredParameters(queryParams.ResultTransformer, aliases);
					}
				}

				for (var i = 0; i < keys.Length; i++)
				{
					if (finalReturnTypes[i] == null)
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
							results[i] = await (PerformAssembleAsync(keys[i], finalReturnTypes[i], queryParams.NaturalKeyLookup, session, (IList) cacheables[i], cancellationToken)).ConfigureAwait(false);
						}
						finally
						{
							persistenceContext.DefaultReadOnly = defaultReadOnlyOrig;
						}
					}
				}

				for (var i = 0; i < keys.Length; i++)
				{
					if (finalReturnTypes[i] == null)
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
							await (InitializeCollectionsAsync(finalReturnTypes[i], session, results[i], (IList) cacheables[i], cancellationToken)).ConfigureAwait(false);
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

		#endregion

		private static async Task<List<object>> GetCacheableResultAsync(
			ICacheAssembler[] returnTypes,
			ISessionImplementor session,
			IList result,
			long ts, string[] aliases, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var cacheable = new List<object>(result.Count + 2) { ts, aliases };
			foreach (var row in result)
			{
				if (returnTypes.Length == 1)
				{
					cacheable.Add(await (returnTypes[0].DisassembleAsync(row, session, null, cancellationToken)).ConfigureAwait(false));
				}
				else
				{
					cacheable.Add(await (TypeHelper.DisassembleAsync((object[])row, returnTypes, null, session, null, cancellationToken)).ConfigureAwait(false));
				}
			}

			return cacheable;
		}

		private static async Task PerformBeforeAssembleAsync(
			ICacheAssembler[] returnTypes,
			ISessionImplementor session,
			IList cacheable, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (returnTypes.Length == 1)
			{
				var returnType = returnTypes[0];

				foreach (var cached in GetResultsEnumerable(cacheable))
				{
					await (returnType.BeforeAssembleAsync(cached, session, cancellationToken)).ConfigureAwait(false);
				}
			}
			else
			{
				foreach (var cached in GetResultsEnumerable(cacheable))
				{
					await (TypeHelper.BeforeAssembleAsync((object[]) cached, returnTypes, session, cancellationToken)).ConfigureAwait(false);
				}
			}
		}

		private async Task<IList> PerformAssembleAsync(
			QueryKey key,
			ICacheAssembler[] returnTypes,
			bool isNaturalKeyLookup,
			ISessionImplementor session,
			IList cacheable, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			try
			{
				var result = new List<object>(cacheable.Count - 2);
				if (returnTypes.Length == 1)
				{
					var returnType = returnTypes[0];

					foreach (var cached in GetResultsEnumerable(cacheable))
					{
						result.Add(await (returnType.AssembleAsync(cached, session, null, cancellationToken)).ConfigureAwait(false));
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
						result.Add(await (TypeHelper.AssembleAsync((object[]) cached, returnTypes, nonCollectionTypeIndexes, session, cancellationToken)).ConfigureAwait(false));
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
					await (Cache.RemoveAsync(key, cancellationToken)).ConfigureAwait(false);
					return null;
				}

				throw;
			}
		}

		private static async Task InitializeCollectionsAsync(
			ICacheAssembler[] returnTypes,
			ISessionImplementor session,
			IList assembleResult,
			IList cacheResult, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
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
				await (TypeHelper.InitializeCollectionsAsync(
					(object[]) cached,
					(object[]) assembleResult[j],
					collectionIndexes,
					session, cancellationToken)).ConfigureAwait(false);
				j++;
			}
		}

		private async Task<IList> GetResultFromCacheableAsync(
			QueryKey key,
			ICacheAssembler[] returnTypes,
			bool isNaturalKeyLookup,
			ISessionImplementor session,
			IList cacheable, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Log.Debug("returning cached query results for: {0}", key);
			returnTypes = GetReturnTypes(key, returnTypes, cacheable);
			try
			{
				session.PersistenceContext.BatchFetchQueue.InitializeQueryCacheQueue();

				await (PerformBeforeAssembleAsync(returnTypes, session, cacheable, cancellationToken)).ConfigureAwait(false);
				var result = await (PerformAssembleAsync(key, returnTypes, isNaturalKeyLookup, session, cacheable, cancellationToken)).ConfigureAwait(false);
				await (InitializeCollectionsAsync(returnTypes, session, result, cacheable, cancellationToken)).ConfigureAwait(false);
				return result;
			}
			finally
			{
				session.PersistenceContext.BatchFetchQueue.TerminateQueryCacheQueue();
			}
		}

		protected virtual Task<bool> IsUpToDateAsync(ISet<string> spaces, long timestamp, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			if (spaces.Count == 0)
				return Task.FromResult<bool>(true);

			return _updateTimestampsCache.IsUpToDateAsync(spaces, timestamp, cancellationToken);
		}
	}
}
