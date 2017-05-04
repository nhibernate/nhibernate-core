using System;
using System.Diagnostics;
using System.Text;
using NHibernate.Cache;
using NHibernate.Cache.Access;
using NHibernate.Cache.Entry;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Type;

namespace NHibernate.Event.Default
{
	/// <summary>
	/// Defines the default load event listeners used by NHibernate for loading entities
	/// in response to generated load events.
	/// </summary>
	[Serializable]
	public partial class DefaultLoadEventListener : AbstractLockUpgradeEventListener, ILoadEventListener
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(DefaultLoadEventListener));
		public static readonly object RemovedEntityMarker = new object();
		public static readonly object InconsistentRTNClassMarker= new object();
		public static readonly LockMode DefaultLockMode = LockMode.None;

		public virtual void OnLoad(LoadEvent @event, LoadType loadType)
		{
			ISessionImplementor source = @event.Session;

			IEntityPersister persister;
			if (@event.InstanceToLoad != null)
			{
				persister = source.GetEntityPersister(null, @event.InstanceToLoad); //the load() which takes an entity does not pass an entityName
				@event.EntityClassName = @event.InstanceToLoad.GetType().FullName;
			}
			else
			{
				persister = GetEntityPersister(source.Factory, @event.EntityClassName);
			}

			if (persister == null)
			{

				var message = new StringBuilder(512);
				message.AppendLine(string.Format("Unable to locate persister for the entity named '{0}'.", @event.EntityClassName));
				message.AppendLine("The persister define the persistence strategy for an entity.");
				message.AppendLine("Possible causes:");
				message.AppendLine(string.Format(" - The mapping for '{0}' was not added to the NHibernate configuration.", @event.EntityClassName));
				throw new HibernateException(message.ToString());
			}

			if (persister.IdentifierType.IsComponentType)
			{
				// skip this check for composite-ids relating to dom4j entity-mode;
				// alternatively, we could add a check to make sure the incoming id value is
				// an instance of Element...
			}
			else
			{
				System.Type idClass = persister.IdentifierType.ReturnedClass;
				if (idClass != null && !idClass.IsInstanceOfType(@event.EntityId))
				{
					throw new TypeMismatchException("Provided id of the wrong type. Expected: " + idClass + ", got " + @event.EntityId.GetType());
				}
			}

			EntityKey keyToLoad = source.GenerateEntityKey(@event.EntityId, persister);
			try
			{
				if (loadType.IsNakedEntityReturned)
				{
					//do not return a proxy!
					//(this option indicates we are initializing a proxy)
					@event.Result = Load(@event, persister, keyToLoad, loadType);
				}
				else
				{
					//return a proxy if appropriate
					if (@event.LockMode == LockMode.None)
					{
						@event.Result = ProxyOrLoad(@event, persister, keyToLoad, loadType);
					}
					else
					{
						@event.Result = LockAndLoad(@event, persister, keyToLoad, loadType, source);
					}
				}
			}
			catch (HibernateException e)
			{
				log.Info("Error performing load command", e);
				throw;
			}
		}

		/// <summary> Perfoms the load of an entity. </summary>
		/// <returns> The loaded entity. </returns>
		protected virtual object Load(LoadEvent @event, IEntityPersister persister, EntityKey keyToLoad, LoadType options)
		{
			if (@event.InstanceToLoad != null)
			{
				if (@event.Session.PersistenceContext.GetEntry(@event.InstanceToLoad) != null)
				{
					throw new PersistentObjectException("attempted to load into an instance that was already associated with the session: " + MessageHelper.InfoString(persister, @event.EntityId, @event.Session.Factory));
				}
				persister.SetIdentifier(@event.InstanceToLoad, @event.EntityId);
			}

			object entity = DoLoad(@event, persister, keyToLoad, options);

			bool isOptionalInstance = @event.InstanceToLoad != null;

			if (!options.IsAllowNulls || isOptionalInstance)
			{
				if (entity == null)
				{
					@event.Session.Factory.EntityNotFoundDelegate.HandleEntityNotFound(@event.EntityClassName, @event.EntityId);
				}
			}

			if (isOptionalInstance && entity != @event.InstanceToLoad)
			{
				throw new NonUniqueObjectException(@event.EntityId, @event.EntityClassName);
			}

			return entity;
		}

		/// <summary>
		/// Based on configured options, will either return a pre-existing proxy,
		/// generate a new proxy, or perform an actual load.
		/// </summary>
		/// <returns> The result of the proxy/load operation.</returns>
		protected virtual object ProxyOrLoad(LoadEvent @event, IEntityPersister persister, EntityKey keyToLoad, LoadType options)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("loading entity: " + MessageHelper.InfoString(persister, @event.EntityId, @event.Session.Factory));
			}

			if (!persister.HasProxy)
			{
				// this class has no proxies (so do a shortcut)
				return Load(@event, persister, keyToLoad, options);
			}
			else
			{
				IPersistenceContext persistenceContext = @event.Session.PersistenceContext;

				// look for a proxy
				object proxy = persistenceContext.GetProxy(keyToLoad);
				if (proxy != null)
				{
					return ReturnNarrowedProxy(@event, persister, keyToLoad, options, persistenceContext, proxy);
				}
				else
				{
					if (options.IsAllowProxyCreation)
					{
						return CreateProxyIfNecessary(@event, persister, keyToLoad, options, persistenceContext);
					}
					else
					{
						// return a newly loaded object
						return Load(@event, persister, keyToLoad, options);
					}
				}
			}
		}

		/// <summary>
		/// Given that there is a pre-existing proxy.
		/// Initialize it if necessary; narrow if necessary.
		/// </summary>
		private object ReturnNarrowedProxy(LoadEvent @event, IEntityPersister persister, EntityKey keyToLoad, LoadType options, IPersistenceContext persistenceContext, object proxy)
		{
			log.Debug("entity proxy found in session cache");
			var castedProxy = (INHibernateProxy) proxy;
			ILazyInitializer li = castedProxy.HibernateLazyInitializer;
			if (li.Unwrap)
			{
				return li.GetImplementation();
			}
			object impl = null;
			if (!options.IsAllowProxyCreation)
			{
				impl = Load(@event, persister, keyToLoad, options);
				// NH Different behavior : NH-1252
				if (impl == null && !options.IsAllowNulls)
				{
					@event.Session.Factory.EntityNotFoundDelegate.HandleEntityNotFound(persister.EntityName, keyToLoad.Identifier);
				}
			}
			if (impl == null && !options.IsAllowProxyCreation && options.ExactPersister)
			{
				// NH Different behavior : NH-1252
				return null;
			}
			return persistenceContext.NarrowProxy(castedProxy, persister, keyToLoad, impl);
		}

		/// <summary>
		/// Given that there is no pre-existing proxy.
		/// Check if the entity is already loaded. If it is, return the entity,
		/// otherwise create and return a proxy.
		/// </summary>
		private object CreateProxyIfNecessary(LoadEvent @event, IEntityPersister persister, EntityKey keyToLoad, LoadType options, IPersistenceContext persistenceContext)
		{
			object existing = persistenceContext.GetEntity(keyToLoad);
			if (existing != null)
			{
				// return existing object or initialized proxy (unless deleted)
				log.Debug("entity found in session cache");
				if (options.IsCheckDeleted)
				{
					EntityEntry entry = persistenceContext.GetEntry(existing);
					Status status = entry.Status;
					if (status == Status.Deleted || status == Status.Gone)
					{
						return null;
					}
				}
				return existing;
			}
			else
			{
				log.Debug("creating new proxy for entity");
				// return new uninitialized proxy
				object proxy = persister.CreateProxy(@event.EntityId, @event.Session);
				persistenceContext.BatchFetchQueue.AddBatchLoadableEntityKey(keyToLoad);
				persistenceContext.AddProxy(keyToLoad, (INHibernateProxy)proxy);
				((INHibernateProxy)proxy)
					.HibernateLazyInitializer
					.ReadOnly = @event.Session.DefaultReadOnly || !persister.IsMutable;
				return proxy;
			}
		}

		/// <summary>
		/// If the class to be loaded has been configured with a cache, then lock
		/// given id in that cache and then perform the load.
		/// </summary>
		/// <returns> The loaded entity </returns>
		protected virtual object LockAndLoad(LoadEvent @event, IEntityPersister persister, EntityKey keyToLoad, LoadType options, ISessionImplementor source)
		{
			ISoftLock sLock = null;
			CacheKey ck;
			if (persister.HasCache)
			{
				ck = source.GenerateCacheKey(@event.EntityId, persister.IdentifierType, persister.RootEntityName);
				sLock = persister.Cache.Lock(ck, null);
			}
			else
			{
				ck = null;
			}

			object entity;
			try
			{
				entity = Load(@event, persister, keyToLoad, options);
			}
			finally
			{
				if (persister.HasCache)
				{
					persister.Cache.Release(ck, sLock);
				}
			}

			object proxy = @event.Session.PersistenceContext.ProxyFor(persister, keyToLoad, entity);

			return proxy;
		}
		/// <summary>
		/// Coordinates the efforts to load a given entity.  First, an attempt is
		/// made to load the entity from the session-level cache.  If not found there,
		/// an attempt is made to locate it in second-level cache.  Lastly, an
		/// attempt is made to load it directly from the datasource.
		/// </summary>
		/// <param name="event">The load event </param>
		/// <param name="persister">The persister for the entity being requested for load </param>
		/// <param name="keyToLoad">The EntityKey representing the entity to be loaded. </param>
		/// <param name="options">The load options. </param>
		/// <returns> The loaded entity, or null. </returns>
		protected virtual object DoLoad(LoadEvent @event, IEntityPersister persister, EntityKey keyToLoad, LoadType options)
		{
			if (log.IsDebugEnabled)
			{
				log.Debug("attempting to resolve: " + MessageHelper.InfoString(persister, @event.EntityId, @event.Session.Factory));
			}

			object entity = LoadFromSessionCache(@event, keyToLoad, options);
			if (entity == RemovedEntityMarker)
			{
				log.Debug("load request found matching entity in context, but it is scheduled for removal; returning null");
				return null;
			}
			if (entity == InconsistentRTNClassMarker)
			{
				log.Debug("load request found matching entity in context, but the matched entity was of an inconsistent return type; returning null");
				return null;
			}
			if (entity != null)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("resolved object in session cache: " + MessageHelper.InfoString(persister, @event.EntityId, @event.Session.Factory));
				}
				return entity;
			}

			entity = LoadFromSecondLevelCache(@event, persister, options);
			if (entity != null)
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("resolved object in second-level cache: " + MessageHelper.InfoString(persister, @event.EntityId, @event.Session.Factory));
				}
				return entity;
			}

			if (log.IsDebugEnabled)
			{
				log.Debug("object not resolved in any cache: " + MessageHelper.InfoString(persister, @event.EntityId, @event.Session.Factory));
			}

			return LoadFromDatasource(@event, persister, keyToLoad, options);
		}

		/// <summary>
		/// Performs the process of loading an entity from the configured underlying datasource.
		/// </summary>
		/// <param name="event">The load event </param>
		/// <param name="persister">The persister for the entity being requested for load </param>
		/// <param name="keyToLoad">The EntityKey representing the entity to be loaded. </param>
		/// <param name="options">The load options. </param>
		/// <returns> The object loaded from the datasource, or null if not found. </returns>
		protected virtual object LoadFromDatasource(LoadEvent @event, IEntityPersister persister, EntityKey keyToLoad, LoadType options)
		{
			ISessionImplementor source = @event.Session;

			bool statsEnabled = source.Factory.Statistics.IsStatisticsEnabled;
			var stopWath = new Stopwatch();
			if (statsEnabled)
			{
				stopWath.Start();
			}

			object entity = persister.Load(@event.EntityId, @event.InstanceToLoad, @event.LockMode, source);

			if (@event.IsAssociationFetch && statsEnabled)
			{
				stopWath.Stop();
				source.Factory.StatisticsImplementor.FetchEntity(@event.EntityClassName, stopWath.Elapsed);
			}

			return entity;
		}

		/// <summary>
		/// Attempts to locate the entity in the session-level cache.
		/// </summary>
		/// <param name="event">The load event </param>
		/// <param name="keyToLoad">The EntityKey representing the entity to be loaded. </param>
		/// <param name="options">The load options. </param>
		/// <returns> The entity from the session-level cache, or null. </returns>
		/// <remarks>
		/// If allowed to return nulls, then if the entity happens to be found in
		/// the session cache, we check the entity type for proper handling
		/// of entity hierarchies.
		/// If checkDeleted was set to true, then if the entity is found in the
		/// session-level cache, it's current status within the session cache
		/// is checked to see if it has previously been scheduled for deletion.
		/// </remarks>
		protected virtual object LoadFromSessionCache(LoadEvent @event, EntityKey keyToLoad, LoadType options)
		{
			ISessionImplementor session = @event.Session;
			object old = session.GetEntityUsingInterceptor(keyToLoad);

			if (old != null)
			{
				// this object was already loaded
				EntityEntry oldEntry = session.PersistenceContext.GetEntry(old);
				if (options.IsCheckDeleted)
				{
					Status status = oldEntry.Status;
					if (status == Status.Deleted || status == Status.Gone)
					{
						return RemovedEntityMarker;
					}
				}
				if (options.IsAllowNulls)
				{
					IEntityPersister persister = GetEntityPersister(@event.Session.Factory, @event.EntityClassName);
					if (!persister.IsInstance(old))
					{
						return InconsistentRTNClassMarker;
					}
				}
				UpgradeLock(old, oldEntry, @event.LockMode, session);
			}
			return old;
		}


		/// <summary> Attempts to load the entity from the second-level cache. </summary>
		/// <param name="event">The load event </param>
		/// <param name="persister">The persister for the entity being requested for load </param>
		/// <param name="options">The load options. </param>
		/// <returns> The entity from the second-level cache, or null. </returns>
		protected virtual object LoadFromSecondLevelCache(LoadEvent @event, IEntityPersister persister, LoadType options)
		{
			ISessionImplementor source = @event.Session;
			bool useCache = persister.HasCache && ((source.CacheMode & CacheMode.Get) == CacheMode.Get)
				&& @event.LockMode.LessThan(LockMode.Read);

			if (useCache)
			{
				ISessionFactoryImplementor factory = source.Factory;

				CacheKey ck = source.GenerateCacheKey(@event.EntityId, persister.IdentifierType, persister.RootEntityName);
				object ce = persister.Cache.Get(ck, source.Timestamp);

				if (factory.Statistics.IsStatisticsEnabled)
				{
					if (ce == null)
					{
						factory.StatisticsImplementor.SecondLevelCacheMiss(persister.Cache.RegionName);
						log.DebugFormat("Entity cache miss: {0}", ck);
					}
					else
					{
						factory.StatisticsImplementor.SecondLevelCacheHit(persister.Cache.RegionName);
						log.DebugFormat("Entity cache hit: {0}", ck);
					}
				}

				if (ce != null)
				{
					CacheEntry entry = (CacheEntry) persister.CacheEntryStructure.Destructure(ce, factory);

					// Entity was found in second-level cache...
					// NH: Different behavior (take a look to options.ExactPersister (NH-295))
					if (!options.ExactPersister || persister.EntityMetamodel.SubclassEntityNames.Contains(entry.Subclass))
					{
						return AssembleCacheEntry(entry, @event.EntityId, persister, @event);
					}
				}
			}

			return null;
		}

		private object AssembleCacheEntry(CacheEntry entry, object id, IEntityPersister persister, LoadEvent @event)
		{
			object optionalObject = @event.InstanceToLoad;
			IEventSource session = @event.Session;
			ISessionFactoryImplementor factory = session.Factory;

			if (log.IsDebugEnabled)
			{
				log.Debug("assembling entity from second-level cache: " + MessageHelper.InfoString(persister, id, factory));
			}

			IEntityPersister subclassPersister = factory.GetEntityPersister(entry.Subclass);
			object result = optionalObject ?? session.Instantiate(subclassPersister, id);

			// make it circular-reference safe
			EntityKey entityKey = session.GenerateEntityKey(id, subclassPersister);
			TwoPhaseLoad.AddUninitializedCachedEntity(entityKey, result, subclassPersister, LockMode.None, entry.AreLazyPropertiesUnfetched, entry.Version, session);

			IType[] types = subclassPersister.PropertyTypes;
			object[] values = entry.Assemble(result, id, subclassPersister, session.Interceptor, session); // intializes result by side-effect
			TypeHelper.DeepCopy(values, types, subclassPersister.PropertyUpdateability, values, session);

			object version = Versioning.GetVersion(values, subclassPersister);
			if (log.IsDebugEnabled)
			{
				log.Debug("Cached Version: " + version);
			}

			IPersistenceContext persistenceContext = session.PersistenceContext;
			bool isReadOnly = session.DefaultReadOnly;

			if (persister.IsMutable)
			{
				object proxy = persistenceContext.GetProxy(entityKey);
				if (proxy != null)
				{
					// this is already a proxy for this impl
					// only set the status to read-only if the proxy is read-only
					isReadOnly = ((INHibernateProxy)proxy).HibernateLazyInitializer.ReadOnly;
				}
			}
			else
				isReadOnly = true;
			
			persistenceContext.AddEntry(
				result,
				isReadOnly ? Status.ReadOnly : Status.Loaded,
				values,
				null,
				id,
				version,
				LockMode.None,
				true,
				subclassPersister,
				false,
				entry.AreLazyPropertiesUnfetched);
			
			subclassPersister.AfterInitialize(result, entry.AreLazyPropertiesUnfetched, session);
			persistenceContext.InitializeNonLazyCollections();
			// upgrade the lock if necessary:
			//lock(result, lockMode);

			//PostLoad is needed for EJB3
			//TODO: reuse the PostLoadEvent...
			PostLoadEvent postLoadEvent = new PostLoadEvent(session);
			postLoadEvent.Entity = result;
			postLoadEvent.Id = id;
			postLoadEvent.Persister = persister;

			IPostLoadEventListener[] listeners = session.Listeners.PostLoadEventListeners;
			for (int i = 0; i < listeners.Length; i++)
			{
				listeners[i].OnPostLoad(postLoadEvent);
			}
			return result;
		}

		protected virtual IEntityPersister GetEntityPersister(ISessionFactoryImplementor factory, string entityName)
		{
			// Check for an exact match.
			IEntityPersister persister = factory.TryGetEntityPersister(entityName);
			if (persister != null)
			{
				return persister;
			}

			// Couldn't find persister through exact name, try finding a single implementing class.
			string[] implementors = factory.GetImplementors(entityName);
			if (implementors.Length > 1)
			{
				var messageBuilder = new StringBuilder(512);
				messageBuilder.AppendLine(string.Format("Ambiguous persister for {0} implemented by more than one hierarchy: ",
				                                        entityName));
				Array.ForEach(implementors, s=> messageBuilder.AppendLine(s));

				throw new HibernateException(messageBuilder.ToString());
			}
			if (implementors.Length == 0)
			{
				return null;
			}
			return factory.GetEntityPersister(implementors[0]);
		}
	}
}
