﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Diagnostics;

using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Collection;

namespace NHibernate.Event.Default
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class DefaultInitializeCollectionEventListener : IInitializeCollectionEventListener
	{

		/// <summary> called by a collection that wants to initialize itself</summary>
		public virtual async Task OnInitializeCollectionAsync(InitializeCollectionEvent @event, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IPersistentCollection collection = @event.Collection;
			ISessionImplementor source = @event.Session;

			bool statsEnabled = source.Factory.Statistics.IsStatisticsEnabled;
			var stopWath = new Stopwatch();
			if (statsEnabled)
			{
				stopWath.Start();
			}

			CollectionEntry ce = source.PersistenceContext.GetCollectionEntry(collection);
			if (ce == null)
				throw new HibernateException("collection was evicted");
			if (!collection.WasInitialized)
			{
				if (log.IsDebugEnabled())
				{
					log.Debug("initializing collection {0}", MessageHelper.CollectionInfoString(ce.LoadedPersister, collection, ce.LoadedKey, source));
				}

				log.Debug("checking second-level cache");
				bool foundInCache = await (InitializeCollectionFromCacheAsync(ce.LoadedKey, ce.LoadedPersister, collection, source, cancellationToken)).ConfigureAwait(false);

				if (foundInCache)
				{
					log.Debug("collection initialized from cache");
				}
				else
				{
					log.Debug("collection not cached");
					await (ce.LoadedPersister.InitializeAsync(ce.LoadedKey, source, cancellationToken)).ConfigureAwait(false);
					log.Debug("collection initialized");

					if (statsEnabled)
					{
						stopWath.Stop();
						source.Factory.StatisticsImplementor.FetchCollection(ce.LoadedPersister.Role, stopWath.Elapsed);
					}
				}
			}
		}

		/// <summary> Try to initialize a collection from the cache</summary>
		private async Task<bool> InitializeCollectionFromCacheAsync(
			object collectionKey, ICollectionPersister persister, IPersistentCollection collection,
			ISessionImplementor source, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (!(source.EnabledFilters.Count == 0) && persister.IsAffectedByEnabledFilters(source))
			{
				log.Debug("disregarding cached version (if any) of collection due to enabled filters ");
				return false;
			}

			bool useCache = persister.HasCache && source.CacheMode.HasFlag(CacheMode.Get);

			if (!useCache)
			{
				return false;
			}

			var batchSize = persister.GetBatchSize();
			CollectionEntry[] collectionEntries = null;
			var collectionBatch = source.PersistenceContext.BatchFetchQueue.QueryCacheQueue
			                            ?.GetCollectionBatch(persister, collectionKey, out collectionEntries);
			if ((collectionBatch != null || batchSize > 1) && persister.Cache.PreferMultipleGet())
			{
				// The first item in the array is the item that we want to load
				if (collectionBatch != null)
				{
					if (collectionBatch.Length == 0)
					{
						return false; // The key was already checked
					}

					batchSize = collectionBatch.Length;
				}

				if (collectionBatch == null)
				{
					collectionEntries = new CollectionEntry[batchSize];
					collectionBatch = await (source.PersistenceContext.BatchFetchQueue
					                        .GetCollectionBatchAsync(persister, collectionKey, batchSize, false, collectionEntries, cancellationToken)).ConfigureAwait(false);
				}

				// Ignore null values as the retrieved batch may contains them when there are not enough
				// uninitialized collection in the queue
				var keys = new List<CacheKey>(batchSize);
				for (var i = 0; i < collectionBatch.Length; i++)
				{
					var key = collectionBatch[i];
					if (key == null)
					{
						break;
					}
					keys.Add(source.GenerateCacheKey(key, persister.KeyType, persister.Role));
				}
				var cachedObjects = await (persister.Cache.GetManyAsync(keys.ToArray(), source.Timestamp, cancellationToken)).ConfigureAwait(false);
				for (var i = 1; i < cachedObjects.Length; i++)
				{
					var coll = source.PersistenceContext.BatchFetchQueue.GetBatchLoadableCollection(persister, collectionEntries[i]);
					await (AssembleAsync(keys[i], cachedObjects[i], persister, source, coll, collectionBatch[i], false, cancellationToken)).ConfigureAwait(false);
				}
				return await (AssembleAsync(keys[0], cachedObjects[0], persister, source, collection, collectionKey, true, cancellationToken)).ConfigureAwait(false);
			}

			var cacheKey = source.GenerateCacheKey(collectionKey, persister.KeyType, persister.Role);
			var cachedObject = await (persister.Cache.GetAsync(cacheKey, source.Timestamp, cancellationToken)).ConfigureAwait(false);
			return await (AssembleAsync(cacheKey, cachedObject, persister, source, collection, collectionKey, true, cancellationToken)).ConfigureAwait(false);
		}

		private async Task<bool> AssembleAsync(
			CacheKey ck, object ce, ICollectionPersister persister, ISessionImplementor source,
			IPersistentCollection collection, object collectionKey, bool alterStatistics, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ISessionFactoryImplementor factory = source.Factory;
			if (factory.Statistics.IsStatisticsEnabled && alterStatistics)
			{
				if (ce == null)
				{
					factory.StatisticsImplementor.SecondLevelCacheMiss(persister.Cache.RegionName);
				}
				else
				{
					factory.StatisticsImplementor.SecondLevelCacheHit(persister.Cache.RegionName);
				}
			}

			if (ce == null)
			{
				log.Debug("Collection cache miss: {0}", ck);
			}
			else
			{
				log.Debug("Collection cache hit: {0}", ck);
			}

			if (ce == null)
			{
				return false;
			}
			else
			{
				IPersistenceContext persistenceContext = source.PersistenceContext;

				CollectionCacheEntry cacheEntry = (CollectionCacheEntry) persister.CacheEntryStructure.Destructure(ce, factory);
				await (cacheEntry.AssembleAsync(collection, persister, persistenceContext.GetCollectionOwner(collectionKey, persister), cancellationToken)).ConfigureAwait(false);

				persistenceContext.GetCollectionEntry(collection).PostInitialize(collection, persistenceContext);

				if (collection.HasQueuedOperations)
					collection.ApplyQueuedOperations();
				return true;
			}
		}
	}
}
