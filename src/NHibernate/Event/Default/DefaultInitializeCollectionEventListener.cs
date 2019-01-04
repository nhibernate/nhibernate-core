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
	[Serializable]
	public partial class DefaultInitializeCollectionEventListener : IInitializeCollectionEventListener
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(DefaultInitializeCollectionEventListener));

		/// <summary> called by a collection that wants to initialize itself</summary>
		public virtual void OnInitializeCollection(InitializeCollectionEvent @event)
		{
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
				bool foundInCache = InitializeCollectionFromCache(ce.LoadedKey, ce.LoadedPersister, collection, source);

				if (foundInCache)
				{
					log.Debug("collection initialized from cache");
				}
				else
				{
					log.Debug("collection not cached");
					ce.LoadedPersister.Initialize(ce.LoadedKey, source);
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
		private bool InitializeCollectionFromCache(
			object collectionKey, ICollectionPersister persister, IPersistentCollection collection,
			ISessionImplementor source)
		{
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
			if (batchSize > 1 && persister.Cache.PreferMultipleGet())
			{
				// The first item in the array is the item that we want to load
				CollectionEntry[] collectionEntries = null;
				object[] collectionBatch = null;
				var queryCacheQueue = source.PersistenceContext.BatchFetchQueue.QueryCacheQueue;
				if (queryCacheQueue != null)
				{
					collectionBatch = queryCacheQueue.GetCollectionBatch(persister, collectionKey, out collectionEntries);
					if (collectionBatch != null)
					{
						if (collectionBatch.Length == 0)
						{
							return false; // The key was already checked
						}

						batchSize = collectionBatch.Length;
					}
				}

				if (collectionBatch == null)
				{
					collectionEntries = new CollectionEntry[batchSize];
					collectionBatch = source.PersistenceContext.BatchFetchQueue
					                        .GetCollectionBatch(persister, collectionKey, batchSize, false, collectionEntries);
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
				var cachedObjects = persister.Cache.GetMany(keys.ToArray(), source.Timestamp);
				for (var i = 1; i < cachedObjects.Length; i++)
				{
					var coll = source.PersistenceContext.BatchFetchQueue.GetBatchLoadableCollection(persister, collectionEntries[i]);
					Assemble(keys[i], cachedObjects[i], persister, source, coll, collectionBatch[i], false);
				}
				return Assemble(keys[0], cachedObjects[0], persister, source, collection, collectionKey, true);
			}

			var cacheKey = source.GenerateCacheKey(collectionKey, persister.KeyType, persister.Role);
			var cachedObject = persister.Cache.Get(cacheKey, source.Timestamp);
			return Assemble(cacheKey, cachedObject, persister, source, collection, collectionKey, true);
		}

		private bool Assemble(
			CacheKey ck, object ce, ICollectionPersister persister, ISessionImplementor source,
			IPersistentCollection collection, object collectionKey, bool alterStatistics)
		{
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
				cacheEntry.Assemble(collection, persister, persistenceContext.GetCollectionOwner(collectionKey, persister));

				persistenceContext.GetCollectionEntry(collection).PostInitialize(collection, persistenceContext);

				if (collection.HasQueuedOperations)
					collection.ApplyQueuedOperations();
				return true;
			}
		}
	}
}
