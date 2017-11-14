using System;

using NHibernate.Classic;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Defines the default listener used by Hibernate for handling save-update events. 
	/// </summary>
	[Serializable]
	public partial class DefaultSaveOrUpdateEventListener : AbstractSaveEventListener, ISaveOrUpdateEventListener
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(DefaultSaveOrUpdateEventListener));

		protected override CascadingAction CascadeAction
		{
			get { return CascadingAction.SaveUpdate; }
		}

		public virtual void OnSaveOrUpdate(SaveOrUpdateEvent @event)
		{
			ISessionImplementor source = @event.Session;
			object obj = @event.Entity;
			object requestedId = @event.RequestedId;

			if (requestedId != null)
			{
				//assign the requested id to the proxy, *before* 
				//reassociating the proxy
				if (obj.IsProxy())
				{
					((INHibernateProxy)obj).HibernateLazyInitializer.Identifier = requestedId;
				}
			}

			if (ReassociateIfUninitializedProxy(obj, source))
			{
				log.Debug("reassociated uninitialized proxy");
				// an uninitialized proxy, noop, don't even need to 
				// return an id, since it is never a save()
			}
			else
			{
				//initialize properties of the event:
				object entity = source.PersistenceContext.UnproxyAndReassociate(obj);
				@event.Entity = entity;
				@event.Entry = source.PersistenceContext.GetEntry(entity);
				//return the id in the event object
				@event.ResultId = PerformSaveOrUpdate(@event);
			}
		}

		protected virtual bool ReassociateIfUninitializedProxy(object obj, ISessionImplementor source)
		{
			return source.PersistenceContext.ReassociateIfUninitializedProxy(obj);
		}

		protected virtual object PerformSaveOrUpdate(SaveOrUpdateEvent @event)
		{
			EntityState entityState = GetEntityState(@event.Entity, @event.EntityName, @event.Entry, @event.Session);

			switch (entityState)
			{
				case EntityState.Detached:
					EntityIsDetached(@event);
					return null;

				case EntityState.Persistent:
					return EntityIsPersistent(@event);

				default:  //TRANSIENT or DELETED
					return EntityIsTransient(@event);
			}
		}

		protected virtual object EntityIsPersistent(SaveOrUpdateEvent @event)
		{
			log.Debug("ignoring persistent instance");

			EntityEntry entityEntry = @event.Entry;
			if (entityEntry == null)
			{
				throw new AssertionFailure("entity was transient or detached");
			}
			else
			{
				if (entityEntry.Status == Status.Deleted)
				{
					throw new AssertionFailure("entity was deleted");
				}

				ISessionFactoryImplementor factory = @event.Session.Factory;

				object requestedId = @event.RequestedId;
				object savedId;
				if (requestedId == null)
				{
					savedId = entityEntry.Id;
				}
				else
				{
					if (!entityEntry.Persister.IdentifierType.IsEqual(requestedId, entityEntry.Id))
					{
						throw new PersistentObjectException("object passed to save() was already persistent: " + 
							MessageHelper.InfoString(entityEntry.Persister, requestedId, factory));
					}
					savedId = requestedId;
				}

				if (log.IsDebugEnabled)
				{
					log.Debug("object already associated with session: " + 
						MessageHelper.InfoString(entityEntry.Persister, savedId, factory));
				}

				return savedId;
			}
		}

		/// <summary> 
		/// The given save-update event named a transient entity.
		/// Here, we will perform the save processing. 
		/// </summary>
		/// <param name="event">The save event to be handled. </param>
		/// <returns> The entity's identifier after saving. </returns>
		protected virtual object EntityIsTransient(SaveOrUpdateEvent @event)
		{
			log.Debug("saving transient instance");

			IEventSource source = @event.Session;
			EntityEntry entityEntry = @event.Entry;
			if (entityEntry != null)
			{
				if (entityEntry.Status == Status.Deleted)
				{
					source.ForceFlush(entityEntry);
				}
				else
				{
					throw new AssertionFailure("entity was persistent");
				}
			}

			object id = SaveWithGeneratedOrRequestedId(@event);

			source.PersistenceContext.ReassociateProxy(@event.Entity, id);

			return id;
		}

		/// <summary> 
		/// Save the transient instance, assigning the right identifier 
		/// </summary>
		/// <param name="event">The initiating event. </param>
		/// <returns> The entity's identifier value after saving.</returns>
		protected virtual object SaveWithGeneratedOrRequestedId(SaveOrUpdateEvent @event)
		{
			if (@event.RequestedId == null)
			{
				return SaveWithGeneratedId(@event.Entity, @event.EntityName, null, @event.Session, true);
			}
			else
			{
				return SaveWithRequestedId(@event.Entity, @event.RequestedId, @event.EntityName, null, @event.Session);
			}
		}

		/// <summary> 
		/// The given save-update event named a detached entity.
		/// Here, we will perform the update processing. 
		/// </summary>
		/// <param name="event">The update event to be handled. </param>
		protected virtual void EntityIsDetached(SaveOrUpdateEvent @event)
		{
			log.Debug("updating detached instance");

			if (@event.Session.PersistenceContext.IsEntryFor(@event.Entity))
			{
				//TODO: assertion only, could be optimized away
				throw new AssertionFailure("entity was persistent");
			}

			object entity = @event.Entity;

			IEntityPersister persister = @event.Session.GetEntityPersister(@event.EntityName, entity);

			@event.RequestedId = GetUpdateId(entity, persister, @event.RequestedId);

			PerformUpdate(@event, entity, persister);
		}

		/// <summary> Determine the id to use for updating. </summary>
		/// <param name="entity">The entity. </param>
		/// <param name="persister">The entity persister </param>
		/// <param name="requestedId">The requested identifier </param>
		/// <returns> The id. </returns>
		protected virtual object GetUpdateId(object entity, IEntityPersister persister, object requestedId)
		{
			// use the id assigned to the instance
			object id = persister.GetIdentifier(entity);
			if (id == null)
			{
				// assume this is a newly instantiated transient object
				// which should be saved rather than updated
				throw new TransientObjectException("The given object has a null identifier: " + persister.EntityName);
			}
			else
			{
				return id;
			}
		}

		protected virtual void PerformUpdate(SaveOrUpdateEvent @event, object entity, IEntityPersister persister)
		{
			if (!persister.IsMutable)
			{
				log.Debug("immutable instance passed to PerformUpdate(), locking");
			}

			if (log.IsDebugEnabled)
			{
				log.Debug("updating " + MessageHelper.InfoString(persister, @event.RequestedId, @event.Session.Factory));
			}

			IEventSource source = @event.Session;

			EntityKey key = source.GenerateEntityKey(@event.RequestedId, persister);

			source.PersistenceContext.CheckUniqueness(key, entity);

			if (InvokeUpdateLifecycle(entity, persister, source))
			{
				Reassociate(@event, @event.Entity, @event.RequestedId, persister);
				return;
			}

			// this is a transient object with existing persistent state not loaded by the session
			new OnUpdateVisitor(source, @event.RequestedId, entity).Process(entity, persister);

			//TODO: put this stuff back in to read snapshot from
			//      the second-level cache (needs some extra work)
			/*Object[] cachedState = null;
			
			if ( persister.hasCache() ) {
			CacheEntry entry = (CacheEntry) persister.getCache()
			.get( event.getRequestedId(), source.getTimestamp() );
			cachedState = entry==null ? 
			null : 
			entry.getState(); //TODO: half-assemble this stuff
			}*/

			source.PersistenceContext.AddEntity(
				entity, 
				persister.IsMutable ? Status.Loaded : Status.ReadOnly,
				null, 
				key,
				persister.GetVersion(entity), 
				LockMode.None, 
				true, 
				persister,
				false,
				true);

			//persister.AfterReassociate(entity, source); TODO H3.2 not ported

			if (log.IsDebugEnabled)
			{
				log.Debug("updating " + MessageHelper.InfoString(persister, @event.RequestedId, source.Factory));
			}

			CascadeOnUpdate(@event, persister, entity);
		}

		protected virtual bool InvokeUpdateLifecycle(object entity, IEntityPersister persister, IEventSource source)
		{
			if (persister.ImplementsLifecycle)
			{
				log.Debug("calling onUpdate()");
				if (((ILifecycle)entity).OnUpdate(source) == LifecycleVeto.Veto)
				{
					log.Debug("update vetoed by onUpdate()");
					return true;
				}
			}
			return false;
		}

		/// <summary> 
		/// Handles the calls needed to perform cascades as part of an update request
		/// for the given entity. 
		/// </summary>
		/// <param name="event">The event currently being processed. </param>
		/// <param name="persister">The defined persister for the entity being updated. </param>
		/// <param name="entity">The entity being updated. </param>
		private void CascadeOnUpdate(SaveOrUpdateEvent @event, IEntityPersister persister, object entity)
		{
			IEventSource source = @event.Session;
			source.PersistenceContext.IncrementCascadeLevel();
			try
			{
				new Cascade(CascadingAction.SaveUpdate, CascadePoint.AfterUpdate, source).CascadeOn(persister, entity);
			}
			finally
			{
				source.PersistenceContext.DecrementCascadeLevel();
			}
		}
	}
}
