using System;
using log4net;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Collection;

namespace NHibernate.Event.Default
{
	[Serializable]
	public class DefaultInitializeCollectionEventListener : IInitializeCollectionEventListener
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DefaultInitializeCollectionEventListener));

		/// <summary> called by a collection that wants to initialize itself</summary>
		public void OnInitializeCollection(InitializeCollectionEvent @event)
		{
			IPersistentCollection collection = @event.Collection;
			ISessionImplementor source = @event.Session;

			CollectionEntry ce = source.PersistenceContext.GetCollectionEntry(collection);
			if (ce == null)
				throw new HibernateException("collection was evicted");
			if (!collection.WasInitialized)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("initializing collection " + MessageHelper.InfoString(ce.LoadedPersister, ce.LoadedKey, source.Factory));
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

					if (source.Factory.Statistics.IsStatisticsEnabled)
					{
						source.Factory.StatisticsImplementor.FetchCollection(ce.LoadedPersister.Role);
					}
				}
			}
		}

		/// <summary> Try to initialize a collection from the cache</summary>
		private bool InitializeCollectionFromCache(object id, ICollectionPersister persister, IPersistentCollection collection, ISessionImplementor source)
		{

			if (!(source.EnabledFilters.Count == 0) && persister.IsAffectedByEnabledFilters(source))
			{
				log.Debug("disregarding cached version (if any) of collection due to enabled filters ");
				return false;
			}

			bool useCache = persister.HasCache && ((source.CacheMode & CacheMode.Get) == CacheMode.Get);

			if (!useCache)
			{
				return false;
			}
			else
			{
				ISessionFactoryImplementor factory = source.Factory;

				CacheKey ck = new CacheKey(id, persister.KeyType, persister.Role, factory);
				object ce = persister.Cache.Get(ck, source.Timestamp);

				if (factory.Statistics.IsStatisticsEnabled)
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
					return false;
				}
				else
				{
					IPersistenceContext persistenceContext = source.PersistenceContext;

					// NH Different implementation but similar behavior H3.2 CollectionCacheEntry.Assemble do de same
					collection.InitializeFromCache(persister, ce, persistenceContext.GetCollectionOwner(id, persister));
					collection.AfterInitialize(persister);

					persistenceContext.GetCollectionEntry(collection).PostInitialize(collection);
					return true;
				}
			}
		}
	}
}
