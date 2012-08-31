using System;
using System.Collections;
using log4net;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;
using NHibernate.Tuple.Entity;
using NHibernate.Tuple;
using System.Collections.Generic;

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

		public virtual void OnRefresh(RefreshEvent @event)
		{
			OnRefresh(@event, IdentityMap.Instantiate(10));
		}

		public virtual void OnRefresh(RefreshEvent @event, IDictionary refreshedAlready)
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
				persister = source.GetEntityPersister(null, obj); //refresh() does not pass an entityName
				id = persister.GetIdentifier(obj, source.EntityMode);
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

			// cascade the refresh prior to refreshing this entity
			refreshedAlready[obj] = obj;
			new Cascade(CascadingAction.Refresh, CascadePoint.BeforeRefresh, source).CascadeOn(persister, obj, refreshedAlready);

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

			// NH Different behavior : NH-1601
			// At this point the entity need the real refresh, all elementes of collections are Refreshed,
			// the collection state was evicted, but the PersistentCollection (in the entity state)
			// is associated with a possible previous session.
			new WrapVisitor(source).Process(obj, persister);

            // NH-3253: Forcing simple load to prevent the redundante instance 
            //on session:
            //Ocurre when:
            // -there is a 'one-to-many' 'Parent-Child' relationship;
            // -AND the 'Parent' 'many-to-one' association is ' fetch="select" ';
            // -AND 'Child' is using a 'composite-id' and 'key-many-to-one' to the
            //'Parent'.
            object result;
            if (this.IsReferencedByCompositeId(persister))
                result = persister.Load(id, obj, @event.LockMode, source);

            string previousFetchProfile = source.FetchProfile;
            source.FetchProfile = "refresh";
            result = persister.Load(id, obj, @event.LockMode, source);
            source.FetchProfile = previousFetchProfile;

			// NH Different behavior : we are ignoring transient entities without throw any kind of exception 
			// because a transient entity is "self refreshed"
			if (!ForeignKeys.IsTransient(persister.EntityName, obj, result == null, @event.Session))
				UnresolvableObjectException.ThrowIfNull(result, id, persister.EntityName);
		}

        private Dictionary<EntityMetamodel, bool> IsReferencedByCompositeIdCache = new Dictionary<EntityMetamodel, bool>();
        /// <summary>
        /// Returns <c>true</c> if the entity in <paramref name="persister"/> 
        /// ('root') has a direct or indirect association with another 
        /// entity that is associated back to 'root' through a 'composit-id' and 
        /// 'key-many-to-one'.
        /// </summary>
        /// <param name="persister"></param>
        /// <returns></returns>
        private bool IsReferencedByCompositeId(IEntityPersister persister)
        {
            try
            {
                bool result = false;
                if (IsReferencedByCompositeIdCache.ContainsKey(persister.EntityMetamodel))
                {
                    result = this.IsReferencedByCompositeIdCache[persister.EntityMetamodel];
                }
                else
                {
                    EntityMetamodel em =
                        this.GetReferrerByCompositeId(
                            persister.EntityMetamodel,
                            persister.EntityMetamodel,
                            false,
                            new Iesi.Collections.Generic.HashedSet<EntityMetamodel>());
                    if (em == null)
                        result = false;
                    else
                        result = true;

                    lock (this)
                    {
                        this.IsReferencedByCompositeIdCache[persister.EntityMetamodel] = result;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                log.Error("Unespected ERROR!", ex);
                throw;
            }
        }

        /// <summary>
        /// Recursive Helper for <see cref="IsReferencedByCompositeId(IEntityPersister)"/>.
        /// </summary>
        /// <param name="rootEM"></param>
        /// <param name="nestedEM"></param>
        /// <param name="neestedIsCompositeId"></param>
        /// <param name="visitedList"></param>
        /// <returns></returns>
        private EntityMetamodel GetReferrerByCompositeId(
            EntityMetamodel rootEM,
            EntityMetamodel nestedEM,
            bool neestedIsCompositeId,
            ICollection<EntityMetamodel> visitedList)
        {
            EntityMetamodel emResult = null;

            if (visitedList.Contains(nestedEM))
            {
                return emResult;
            }
            else
            {
                visitedList.Add(nestedEM);

                ISessionFactoryImplementor sessionImplementor = rootEM.SessionFactory;

                if (nestedEM.IdentifierProperty.Type is IAbstractComponentType)
                {
                    IAbstractComponentType componentType = (IAbstractComponentType)nestedEM.IdentifierProperty.Type;
                    for (int i = 0; i < componentType.Subtypes.Length; i++)
                    {
                        IType subType = componentType.Subtypes[i];
                        if (!subType.IsAnyType
                            && subType.IsAssociationType
                            && subType is IAssociationType)
                        {
                            IAssociationType associationType = (IAssociationType)subType;
                            string associatedEntityName = null;
                            try
                            {
                                //for 'Collection Types', sometimes 'Element Type' is not an 'Entity Type'
                                associatedEntityName = associationType.GetAssociatedEntityName(sessionImplementor);
                            }
                            catch (MappingException me)
                            {
                                //I think it will never happen because a 
                                //"Composit Id" can not have a property that 
                                //uses 'NHibernate.Type.CollectionType'. 
                                //But just in case ...
                                if (log.IsDebugEnabled)
                                    log.Debug("Can not perform 'GetAssociatedEntityName'. " +
                                        "Considering it is not an entity type: '" +
                                        nestedEM.IdentifierProperty.Name + "." +
                                        componentType.PropertyNames[i] + "'"
                                        , me);
                            }
                            if (associatedEntityName != null)
                            {
                                IEntityPersister persisterNextNested = sessionImplementor.GetEntityPersister(associatedEntityName);
                                if (rootEM == persisterNextNested.EntityMetamodel)
                                {
                                    emResult = nestedEM;
                                    return emResult;
                                }
                                else
                                {
                                    emResult = this.GetReferrerByCompositeId(
                                        rootEM,
                                        persisterNextNested.EntityMetamodel,
                                        true,
                                        visitedList);

                                    if (emResult != null)
                                        return emResult;
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < nestedEM.Properties.Length; i++)
                {
                    StandardProperty property = nestedEM.Properties[i];

                    if (!property.Type.IsAnyType
                        && property.Type.IsAssociationType
                        && property.Type is IAssociationType)
                    {
                        IAssociationType associationType = (IAssociationType)property.Type;
                        string associatedEntityName = null;
                        try
                        {
                            //for 'Collection Types', sometimes 'Element Type' is not an 'Entity Type'
                            associatedEntityName = associationType.GetAssociatedEntityName(sessionImplementor);
                        }
                        catch (MappingException me)
                        {
                            if (log.IsDebugEnabled)
                                log.Debug("Can not perform 'GetAssociatedEntityName'. " +
                                        "Considering it is not an entity type: '" +
                                        nestedEM.EntityType.Name + "." +
                                        nestedEM.PropertyNames[i] + "'",
                                    me);
                        }
                        if (associatedEntityName != null)
                        {
                            IEntityPersister persisterNextNested = sessionImplementor.GetEntityPersister(associatedEntityName);
                            emResult = this.GetReferrerByCompositeId(
                                rootEM,
                                persisterNextNested.EntityMetamodel,
                                false,
                                visitedList);
                            if (emResult != null)
                                return emResult;
                        }
                    }
                }
            }

            return null;
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
