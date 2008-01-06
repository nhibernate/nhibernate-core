using System;
using System.Collections;
using log4net;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Defines the default refresh event listener used by hibernate for refreshing entities
	/// in response to generated refresh events. 
	/// </summary>
	[Serializable]
	public class DefaultRefreshEventListener : IRefreshEventListener
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DefaultRefreshEventListener));

		public void OnRefresh(RefreshEvent @event)
		{
			OnRefresh(@event, IdentityMap.Instantiate(10));
		}

		public void OnRefresh(RefreshEvent @event, IDictionary refreshedAlready)
		{
			IEventSource source = @event.Session;

			if (source.PersistenceContext.ReassociateIfUninitializedProxy(@event.Entity))
				return;

			object obj = source.PersistenceContext.UnproxyAndReassociate(@event.Entity);

			if (refreshedAlready.Contains(obj))
			{
				log.Debug("already refreshed");
				return;
			}

			EntityEntry e = source.PersistenceContext.GetEntry(obj);
			IEntityPersister persister;
			object id;

			if (e == null)
			{
				persister = source.GetEntityPersister(obj); //refresh() does not pass an entityName
				id = persister.GetIdentifier(obj);
				if (log.IsDebugEnabled)
				{
					log.Debug("refreshing transient " + MessageHelper.InfoString(persister, id, source.Factory));
				}
				EntityKey key = new EntityKey(id, persister, source.EntityMode);
				if (source.PersistenceContext.GetEntry(key) != null)
				{
					throw new PersistentObjectException("attempted to refresh transient instance when persistent instance was already associated with the Session: " + 
						MessageHelper.InfoString(persister, id, source.Factory));
				}
			}
			else
			{
				if (log.IsDebugEnabled)
				{
					log.Debug("refreshing " + MessageHelper.InfoString(e.Persister, e.Id, source.Factory));
				}
				if (!e.ExistsInDatabase)
				{
					throw new HibernateException("this instance does not yet exist as a row in the database");
				}

				persister = e.Persister;
				id = e.Id;
			}

			// NH Different behavior (H3.2 the cascade is explicit in events; NH are implicit in loader and so on)
			// cascade the refresh prior to refreshing this entity
			//refreshedAlready[obj] = obj;
			//Cascades.Cascade(source, persister, obj, Cascades.CascadingAction.ActionRefresh, CascadePoint.CascadeBeforeRefresh,
			//                 refreshedAlready);

			if (e != null)
			{
				EntityKey key = new EntityKey(id, persister, source.EntityMode);
				source.PersistenceContext.RemoveEntity(key);
				if (persister.HasCollections)
					new EvictVisitor(source).Process(obj, persister);
			}

			if (persister.HasCache)
			{
				CacheKey ck = new CacheKey(id, persister.IdentifierType, persister.RootEntityName, source.EntityMode, source.Factory);
				persister.Cache.Remove(ck);
			}

			EvictCachedCollections(persister, id, source.Factory);

			// todo-events Different behaviour
			//string previousFetchProfile = source.FetchProfile;
			//source.FetchProfile = "refresh";
			object result = persister.Load(id, obj, @event.LockMode, source);
			//source.FetchProfile = previousFetchProfile;

			UnresolvableObjectException.ThrowIfNull(result, id, persister.MappedClass);
		}

		// Evict collections from the factory-level cache
		private void EvictCachedCollections(IEntityPersister persister, object id, ISessionFactoryImplementor factory)
		{
			EvictCachedCollections(persister.PropertyTypes, id, factory);
		}

		private void EvictCachedCollections(IType[] types, object id, ISessionFactoryImplementor factory)
		{
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].IsCollectionType)
				{
					factory.EvictCollection(((CollectionType)types[i]).Role, id);
				}
				else if (types[i].IsComponentType)
				{
					IAbstractComponentType actype = (IAbstractComponentType)types[i];
					EvictCachedCollections(actype.Subtypes, id, factory);
				}
			}
		}
	}
}
