using log4net;
using NHibernate.Cache;
using NHibernate.Event;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary> 
	/// Functionality relating to Hibernate's two-phase loading process,
	/// that may be reused by persisters that do not use the Loader
	/// framework
	/// </summary>
	public static class TwoPhaseLoad
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(TwoPhaseLoad));
		
		/// <summary> 
		/// Register the "hydrated" state of an entity instance, after the first step of 2-phase loading.
		/// 
		/// Add the "hydrated state" (an array) of an uninitialized entity to the session. We don't try
		/// to resolve any associations yet, because there might be other entities waiting to be
		/// read from the JDBC result set we are currently processing
		/// </summary>
		public static void PostHydrate(IEntityPersister persister, object id, object[] values, object obj, LockMode lockMode, bool lazyPropertiesAreUnfetched, ISessionImplementor session)
		{
			object version = Versioning.GetVersion(values, persister);
			session.PersistenceContext.AddEntry(obj, Status.Loading, values, id, version, lockMode, true, persister, false, lazyPropertiesAreUnfetched);

			if (log.IsDebugEnabled && version != null)
			{
				string versionStr = persister.IsVersioned ? persister.VersionType.ToLoggableString(version, session.Factory) : "null";
				log.Debug("Version: " + versionStr);
			}
		}

		/// <summary> 
		/// Perform the second step of 2-phase load. Fully initialize the entity instance. 
		/// After processing a JDBC result set, we "resolve" all the associations
		/// between the entities which were instantiated and had their state
		/// "hydrated" into an array
		/// </summary>
		public static void InitializeEntity(object entity, bool readOnly, ISessionImplementor session, PreLoadEvent preLoadEvent, PostLoadEvent postLoadEvent)
		{
			//TODO: Should this be an InitializeEntityEventListener??? (watch out for performance!)
			IPersistenceContext persistenceContext = session.PersistenceContext;
			EntityEntry entityEntry = persistenceContext.GetEntry(entity);
			if (entityEntry == null)
			{
				throw new AssertionFailure("possible non-threadsafe access to the session");
			}
			IEntityPersister persister = entityEntry.Persister;
			object id = entityEntry.Id;
			object[] hydratedState = entityEntry.LoadedState;

			if (log.IsDebugEnabled)
				log.Debug("resolving associations for " + MessageHelper.InfoString(persister, id, session.Factory));

			IType[] types = persister.PropertyTypes;
			for (int i = 0; i < hydratedState.Length; i++)
			{
				hydratedState[i] = types[i].ResolveIdentifier(hydratedState[i], session, entity);

				//object value = hydratedState[i];
				//if (value != org.hibernate.intercept.LazyPropertyInitializer.UNFETCHED_PROPERTY && value != BackrefPropertyAccessor.UNKNOWN)
				//{
				//hydratedState[i] = types[i].ResolveIdentifier(value, session, entity);
				//}
			}

			//Must occur after resolving identifiers!
			if (session.IsEventSource)
			{
				preLoadEvent.Entity = entity;
				preLoadEvent.State = hydratedState;
				preLoadEvent.Id = id;
				preLoadEvent.Persister=persister;
				IPreLoadEventListener[] listeners = session.Listeners.PreLoadEventListeners;
				for (int i = 0; i < listeners.Length; i++)
				{
					listeners[i].OnPreLoad(preLoadEvent);
				}
			}

			persister.SetPropertyValues(entity, hydratedState);

			//if (persister.HasCache && session.CacheMode.PutEnabled) TODO H3.2 Different behaviour
			if (persister.HasCache)
			{
				if (log.IsDebugEnabled)
					log.Debug("adding entity to second-level cache: " + MessageHelper.InfoString(persister, id, session.Factory));

				object version = Versioning.GetVersion(hydratedState, persister);
				CacheEntry entry =
					new CacheEntry(entity, persister, session);
				CacheKey cacheKey =
					new CacheKey(id, persister.IdentifierType, persister.RootEntityName, session.Factory);
				persister.Cache.Put(cacheKey, entry, session.Timestamp, version,
				                    persister.IsVersioned ? persister.VersionType.Comparator : null,
				                    UseMinimalPuts(session, entityEntry));
					//we could use persister.hasLazyProperties() instead of true

				// TODO H3.2 Not ported
				//if (put && factory.Statistics.StatisticsEnabled)
				//{
				//  factory.StatisticsImplementor.secondLevelCachePut(persister.Cache.RegionName);
				//}
			}

			if (readOnly || !persister.IsMutable)
			{
				//no need to take a snapshot - this is a 
				//performance optimization, but not really
				//important, except for entities with huge 
				//mutable property values
				persistenceContext.SetEntryStatus(entityEntry, Status.ReadOnly);
			}
			else
			{
				//take a snapshot
				TypeFactory.DeepCopy(hydratedState, persister.PropertyTypes, persister.PropertyUpdateability, hydratedState);
				persistenceContext.SetEntryStatus(entityEntry, Status.Loaded);
			}

			// TODO H3.2 properties lazyness
			//persister.AfterInitialize(entity, entityEntry.LoadedWithLazyPropertiesUnfetched, session);

			if (session.IsEventSource)
			{
				postLoadEvent.Entity = entity;
				postLoadEvent.Id = id;
				postLoadEvent.Persister = persister;
				IPostLoadEventListener[] listeners = session.Listeners.PostLoadEventListeners;
				for (int i = 0; i < listeners.Length; i++)
				{
					listeners[i].OnPostLoad(postLoadEvent);
				}
			}

			if (log.IsDebugEnabled)
				log.Debug("done materializing entity " + MessageHelper.InfoString(persister, id, session.Factory));

			// TODO H3.2 Not ported
			//if (factory.Statistics.StatisticsEnabled)
			//{
			//  factory.StatisticsImplementor.loadEntity(persister.EntityName);
			//}
		}

		private static bool UseMinimalPuts(ISessionImplementor session, EntityEntry entityEntry)
		{
			return session.Factory.Settings.IsMinimalPutsEnabled;
			// TODO H3.2 Different behaviour property lazyness
			//return (session.Factory.Settings.IsMinimalPutsEnabled && session.CacheMode != CacheMode.REFRESH)
			//|| (entityEntry.Persister.hasLazyProperties() && entityEntry.LoadedWithLazyPropertiesUnfetched && entityEntry.Persister.LazyPropertiesCacheable);
		}

		/// <summary> 
		/// Add an uninitialized instance of an entity class, as a placeholder to ensure object 
		/// identity. Must be called before <tt>postHydrate()</tt>.
		///  Create a "temporary" entry for a newly instantiated entity. The entity is uninitialized,
		/// but we need the mapping from id to instance in order to guarantee uniqueness.
		/// </summary>
		public static void AddUninitializedEntity(EntityKey key, object obj, IEntityPersister persister, LockMode lockMode, bool lazyPropertiesAreUnfetched, ISessionImplementor session)
		{
			session.PersistenceContext.AddEntity(obj, Status.Loading, null, key, null, lockMode, true, persister, false, lazyPropertiesAreUnfetched);
		}

		public static void AddUninitializedCachedEntity(EntityKey key, object obj, IEntityPersister persister, LockMode lockMode, bool lazyPropertiesAreUnfetched, object version, ISessionImplementor session)
		{
			session.PersistenceContext.AddEntity(obj, Status.Loading, null, key, version, lockMode, true, persister, false, lazyPropertiesAreUnfetched);
		}
	}
}
