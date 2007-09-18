using System;
using System.Collections;
using log4net;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Defines the default copy event listener used by hibernate for copying entities
	/// in response to generated copy events. 
	/// </summary>
	[Serializable]
	public class DefaultMergeEventListener : AbstractSaveEventListener, IMergeEventListener
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DefaultMergeEventListener));

		protected internal override Cascades.CascadingAction CascadeAction
		{
			get { return Cascades.CascadingAction.ActionCopy; }
		}

		protected internal override bool? AssumedUnsaved
		{
			get { return false; }
		}

		protected internal override IDictionary GetMergeMap(object anything)
		{
			return IdentityMap.Invert((IDictionary)anything);
		}

		public void OnMerge(MergeEvent theEvent)
		{
			OnMerge(theEvent, IdentityMap.Instantiate(10));
		}

		public void OnMerge(MergeEvent theEvent, IDictionary copyCache)
		{
			IEventSource source = theEvent.Session;
			object original = theEvent.Original;

			if (original != null)
			{
				object entity;
				if (original is INHibernateProxy)
				{
					LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy)original);
					if (li.IsUninitialized)
					{
						log.Debug("ignoring uninitialized proxy");
						theEvent.Result = source.Load(li.PersistentClass, li.Identifier);
						return; //EARLY EXIT!
					}
					else
					{
						entity = li.GetImplementation();
					}
				}
				else
				{
					entity = original;
				}

				if (copyCache.Contains(entity))
				{
					log.Debug("already merged");
					theEvent.Result = entity;
				}
				else
				{
					theEvent.Entity = entity;
					EntityState entityState = EntityState.Undefined;

					// Check the persistence context for an entry relating to this
					// entity to be merged...
					EntityEntry entry = source.GetEntry(entity);
					if (entry == null)
					{
						IEntityPersister persister = source.GetEntityPersister(entity);
						object id = persister.GetIdentifier(entity);
						if (id != null)
						{
							EntityKey key = new EntityKey(id, persister);
							object managedEntity = source.GetEntity(key);
							entry = source.GetEntry(managedEntity);
							if (entry != null)
							{
								// we have specialized case of a detached entity from the
								// perspective of the merge operation.  Specifically, we
								// have an incoming entity instance which has a corresponding
								// entry in the current persistence context, but registered
								// under a different entity instance
								entityState = EntityState.Detached;
							}
						}
					}

					if (entityState == EntityState.Undefined)
					{
						entityState = GetEntityState(entity, theEvent.EntityName, entry, source);
					}

					switch (entityState)
					{
						case EntityState.Persistent:
							EntityIsPersistent(theEvent, copyCache);
							break;
						case EntityState.Transient:
							EntityIsTransient(theEvent, copyCache);
							break;
						case EntityState.Detached:
							EntityIsDetached(theEvent, copyCache);
							break;
						default:
							throw new ObjectDeletedException("deleted instance passed to merge", null, entity.GetType());
					}
				}
			}
		}

		protected internal void EntityIsPersistent(MergeEvent @event, IDictionary copyCache)
		{
			log.Debug("ignoring persistent instance");

			//TODO: check that entry.getIdentifier().equals(requestedId)
			object entity = @event.Entity;
			IEventSource source = @event.Session;
			IEntityPersister persister = source.GetEntityPersister(entity);

			copyCache[entity] = entity; //before cascade!

			CascadeOnMerge(source, persister, entity, copyCache);
			CopyValues(persister, entity, entity, source, copyCache);

			@event.Result = entity;
		}

		protected internal void EntityIsTransient(MergeEvent @event, IDictionary copyCache)
		{
			log.Debug("merging transient instance");

			object entity = @event.Entity;
			IEventSource source = @event.Session;

			IEntityPersister persister = source.GetEntityPersister(entity);
			string entityName = persister.EntityName;

			object id = persister.HasIdentifierProperty ? persister.GetIdentifier(entity) : null;

			object copy = persister.Instantiate(id); //TODO: should this be Session.instantiate(Persister, ...)?
			copyCache[entity] = copy; //before cascade!

			// cascade first, so that all unsaved objects get their
			// copy created before we actually copy
			//cascadeOnMerge(event, persister, entity, copyCache, Cascades.CASCADE_BEFORE_MERGE);
			base.CascadeBeforeSave(source, persister, entity, copyCache);
			CopyValues(persister, entity, copy, source, copyCache, ForeignKeyDirection.ForeignKeyFromParent);

			//this bit is only *really* absolutely necessary for handling 
			//requestedId, but is also good if we merge multiple object 
			//graphs, since it helps ensure uniqueness
			object requestedId = @event.RequestedId;
			if (requestedId == null)
			{
				SaveWithGeneratedId(copy, entityName, copyCache, source, false);
			}
			else
			{
				SaveWithRequestedId(copy, requestedId, entityName, copyCache, source);
			}

			// cascade first, so that all unsaved objects get their 
			// copy created before we actually copy
			base.CascadeAfterSave(source, persister, entity, copyCache);
			CopyValues(persister, entity, copy, source, copyCache, ForeignKeyDirection.ForeignKeyToParent);

			@event.Result = copy;
		}

		protected internal void EntityIsDetached(MergeEvent @event, IDictionary copyCache)
		{
			log.Debug("merging detached instance");

			object entity = @event.Entity;
			IEventSource source = @event.Session;

			IEntityPersister persister = source.GetEntityPersister(entity);
			string entityName = persister.EntityName;

			object id = @event.RequestedId;
			if (id == null)
			{
				id = persister.GetIdentifier(entity);
			}
			else
			{
				// check that entity id = requestedId
				object entityId = persister.GetIdentifier(entity);
				if (!persister.IdentifierType.Equals(id, entityId))
				{
					throw new HibernateException("merge requested with id not matching id of passed entity");
				}
			}

			// TODO H3.2 Not ported
			//string previousFetchProfile = source.FetchProfile;
			//source.FetchProfile = "merge";

			//we must clone embedded composite identifiers, or 
			//we will get back the same instance that we pass in
			object clonedIdentifier = persister.IdentifierType.DeepCopy(id);
			object result = source.Get(persister.MappedClass, clonedIdentifier);

			//source.FetchProfile = previousFetchProfile;

			if (result == null)
			{
				//TODO: we should throw an exception if we really *know* for sure  
				//      that this is a detached instance, rather than just assuming
				//throw new StaleObjectStateException(entityName, id);

				// we got here because we assumed that an instance
				// with an assigned id was detached, when it was
				// really persistent
				EntityIsTransient(@event, copyCache);
			}
			else
			{
				copyCache[entity] = result; //before cascade!

				object target = source.Unproxy(result);
				if (target == entity)
				{
					throw new AssertionFailure("entity was not detached");
				}
				else if (!source.GetEntityName(target).Equals(entityName))
				{
					throw new WrongClassException("class of the given object did not match class of persistent copy", @event.RequestedId, persister.MappedClass);
				}
				else if (IsVersionChanged(entity, source, persister, target))
				{
					// TODO H3.2 Not ported
					//if (source.Factory.Statistics.StatisticsEnabled)
					//{
					//  source.Factory.StatisticsImplementor.optimisticFailure(entityName);
					//}
					throw new StaleObjectStateException(persister.MappedClass, id);
				}

				// cascade first, so that all unsaved objects get their 
				// copy created before we actually copy
				CascadeOnMerge(source, persister, entity, copyCache);
				CopyValues(persister, entity, target, source, copyCache);

				//copyValues works by reflection, so explicitly mark the entity instance dirty
				MarkInterceptorDirty(entity, target);

				@event.Result = result;
			}
		}

		private void MarkInterceptorDirty(object entity, object target)
		{
			// TODO H3.2 not ported
			//if (FieldInterceptionHelper.isInstrumented(entity))
			//{
			//  FieldInterceptor interceptor = FieldInterceptionHelper.extractFieldInterceptor(target);
			//  if (interceptor != null)
			//  {
			//    interceptor.dirty();
			//  }
			//}
		}

		private static bool IsVersionChanged(object entity, IEventSource source, IEntityPersister persister, object target)
		{
			if (!persister.IsVersioned)
			{
				return false;
			}
			// for merging of versioned entities, we consider the version having
			// been changed only when:
			// 1) the two version values are different;
			//      *AND*
			// 2) The target actually represents database state!
			//
			// This second condition is a special case which allows
			// an entity to be merged during the same transaction
			// (though during a seperate operation) in which it was
			// originally persisted/saved
			// todo-events Verify if "IsEquals" in NH work like "IsSame" in H3.2
			bool changed = !persister.VersionType.Equals(persister.GetVersion(target), persister.GetVersion(entity));

			// TODO : perhaps we should additionally require that the incoming entity
			// version be equivalent to the defined unsaved-value?
			return changed && ExistsInDatabase(target, source, persister);
		}

		private static bool ExistsInDatabase(object entity, IEventSource source, IEntityPersister persister)
		{
			EntityEntry entry = source.GetEntry(entity);
			if (entry == null)
			{
				object id = persister.GetIdentifier(entity);
				if (id != null)
				{
					EntityKey key = new EntityKey(id, persister);
					object managedEntity = source.GetEntity(key);
					entry = source.GetEntry(managedEntity);
				}
			}

			if (entry == null)
			{
				// perhaps this should be an exception since it is only ever used
				// in the above method?
				return false;
			}
			else
			{
				return entry.ExistsInDatabase;
			}
		}

		protected internal static void CopyValues(IEntityPersister persister, object entity, object target, ISessionImplementor source, IDictionary copyCache)
		{
			object[] copiedValues = TypeFactory.Replace(
				persister.GetPropertyValues(entity), persister.GetPropertyValues(target), 
				persister.PropertyTypes, source, target, copyCache);

			persister.SetPropertyValues(target, copiedValues);
		}

		protected internal static void CopyValues(IEntityPersister persister, object entity, object target, ISessionImplementor source, IDictionary copyCache, ForeignKeyDirection foreignKeyDirection)
		{
			object[] copiedValues;

			if (foreignKeyDirection == ForeignKeyDirection.ForeignKeyToParent)
			{
				// this is the second pass through on a merge op, so here we limit the
				// replacement to associations types (value types were already replaced
				// during the first pass)
				copiedValues = TypeFactory.ReplaceAssociations(persister.GetPropertyValues(entity), 
					persister.GetPropertyValues(target), 
					persister.PropertyTypes, source, target, copyCache, foreignKeyDirection);
			}
			else
			{
				copiedValues = TypeFactory.Replace(persister.GetPropertyValues(entity), 
					persister.GetPropertyValues(target), 
					persister.PropertyTypes, source, target, copyCache, foreignKeyDirection);
			}

			persister.SetPropertyValues(target, copiedValues);
		}

		/// <summary> 
		/// Perform any cascades needed as part of this copy event.
		/// </summary>
		/// <param name="source">The merge event being processed. </param>
		/// <param name="persister">The persister of the entity being copied. </param>
		/// <param name="entity">The entity being copied. </param>
		/// <param name="copyCache">A cache of already copied instance. </param>
		protected internal void CascadeOnMerge(IEventSource source, IEntityPersister persister, object entity, IDictionary copyCache)
		{
			source.IncrementCascadeLevel();
			try
			{
				Cascades.Cascade(source, persister, entity, CascadeAction, CascadePoint.CascadeOnCopy, copyCache);
			}
			finally
			{
				source.DecrementCascadeLevel();
			}
		}
		
		/// <summary> Cascade behavior is redefined by this subclass, disable superclass behavior</summary>
		protected internal override void CascadeAfterSave(IEventSource source, IEntityPersister persister, object entity, object anything)
		{
		}

		/// <summary> Cascade behavior is redefined by this subclass, disable superclass behavior</summary>
		protected internal override void CascadeBeforeSave(IEventSource source, IEntityPersister persister, object entity, object anything)
		{
		}
	}
}
