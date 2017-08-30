using System;
using System.Collections;

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
	public partial class DefaultRefreshEventListener : IRefreshEventListener
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(DefaultRefreshEventListener));

		public virtual void OnRefresh(RefreshEvent @event)
		{
			OnRefresh(@event, IdentityMap.Instantiate(10));
		}

		public virtual void OnRefresh(RefreshEvent @event, IDictionary refreshedAlready)
		{
			IEventSource source = @event.Session;

			bool isTransient = !source.Contains(@event.Entity);
			if (source.PersistenceContext.ReassociateIfUninitializedProxy(@event.Entity))
			{
				if (isTransient)
					source.SetReadOnly(@event.Entity, source.DefaultReadOnly);
				return;
			}

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
				persister = source.GetEntityPersister(null, obj); //refresh() does not pass an entityName
				id = persister.GetIdentifier(obj);
				if (log.IsDebugEnabled)
				{
					log.Debug("refreshing transient " + MessageHelper.InfoString(persister, id, source.Factory));
				}
				EntityKey key = source.GenerateEntityKey(id, persister);
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

			// cascade the refresh prior to refreshing this entity
			refreshedAlready[obj] = obj;
			new Cascade(CascadingAction.Refresh, CascadePoint.BeforeRefresh, source).CascadeOn(persister, obj, refreshedAlready);

			if (e != null)
			{
				EntityKey key = source.GenerateEntityKey(id, persister);
				source.PersistenceContext.RemoveEntity(key);
				if (persister.HasCollections)
					new EvictVisitor(source).Process(obj, persister);
			}

			if (persister.HasCache)
			{
				CacheKey ck = source.GenerateCacheKey(id, persister.IdentifierType, persister.RootEntityName);
				persister.Cache.Remove(ck);
			}

			EvictCachedCollections(persister, id, source.Factory);

			// NH Different behavior : NH-1601
			// At this point the entity need the real refresh, all elementes of collections are Refreshed,
			// the collection state was evicted, but the PersistentCollection (in the entity state)
			// is associated with a possible previous session.
			new WrapVisitor(source).Process(obj, persister);

			string previousFetchProfile = source.FetchProfile;
			source.FetchProfile = "refresh";
			object result = persister.Load(id, obj, @event.LockMode, source);
			
			if (result != null)
				if (!persister.IsMutable)
					source.SetReadOnly(result, true);
				else
					source.SetReadOnly(result, (e == null ? source.DefaultReadOnly : e.IsReadOnly));
			
			source.FetchProfile = previousFetchProfile;

			// NH Different behavior : we are ignoring transient entities without throw any kind of exception
			// because a transient entity is "self refreshed"
			if (!ForeignKeys.IsTransientFast(persister.EntityName, obj, @event.Session).GetValueOrDefault(result == null))
				UnresolvableObjectException.ThrowIfNull(result, id, persister.EntityName);
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
