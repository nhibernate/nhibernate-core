using System;
using System.Collections;

using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Util;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Defines the default create event listener used by hibernate for creating
	/// transient entities in response to generated create events. 
	/// </summary>
	[Serializable]
	public partial class DefaultPersistEventListener : AbstractSaveEventListener, IPersistEventListener
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(DefaultPersistEventListener));

		protected override CascadingAction CascadeAction
		{
			get { return CascadingAction.Persist; }
		}

		protected override bool? AssumedUnsaved
		{
			get { return true; }
		}

		public virtual void OnPersist(PersistEvent @event)
		{
			OnPersist(@event, IdentityMap.Instantiate(10));
		}

		public virtual void OnPersist(PersistEvent @event, IDictionary createdAlready)
		{
			ISessionImplementor source = @event.Session;
			object obj = @event.Entity;

			object entity;
			if (obj.IsProxy())
			{
				ILazyInitializer li = ((INHibernateProxy)obj).HibernateLazyInitializer;
				if (li.IsUninitialized)
				{
					if (li.Session == source)
					{
						return; //NOTE EARLY EXIT!
					}
					else
					{
						throw new PersistentObjectException("uninitialized proxy passed to persist()");
					}
				}
				entity = li.GetImplementation();
			}
			else
			{
				entity = obj;
			}

			EntityState entityState = GetEntityState(entity, @event.EntityName, source.PersistenceContext.GetEntry(entity), source);

			switch (entityState)
			{
				case EntityState.Persistent:
					EntityIsPersistent(@event, createdAlready);
					break;
				case EntityState.Transient:
					EntityIsTransient(@event, createdAlready);
					break;
				case EntityState.Detached:
					throw new PersistentObjectException("detached entity passed to persist: " + GetLoggableName(@event.EntityName, entity));
				default:
					throw new ObjectDeletedException("deleted instance passed to merge", null, GetLoggableName(@event.EntityName, entity));
			}
		}

		protected virtual void EntityIsPersistent(PersistEvent @event, IDictionary createCache)
		{
			log.Debug("ignoring persistent instance");
			IEventSource source = @event.Session;

			//TODO: check that entry.getIdentifier().equals(requestedId)
			object entity = source.PersistenceContext.Unproxy(@event.Entity);
			/* NH-2565: the UnProxy may return a "field interceptor proxy". When EntityName is null the session.GetEntityPersister will try to guess it.
			 * Instead change a session's method I'll try to guess the EntityName here.
			 * Because I'm using a session's method perhaps could be better if each session's method, which implementation forward to a method having the EntityName as parameter,
			 * use the BestGuessEntityName directly instead do "70 turns" before call it.
			*/
			if (@event.EntityName == null)
			{
				@event.EntityName = source.BestGuessEntityName(entity);
			}
			IEntityPersister persister = source.GetEntityPersister(@event.EntityName, entity);

			object tempObject;
			tempObject = createCache[entity];
			createCache[entity] = entity;
			if (tempObject == null)
			{
				//TODO: merge into one method!
				CascadeBeforeSave(source, persister, entity, createCache);
				CascadeAfterSave(source, persister, entity, createCache);
			}
		}

		/// <summary> Handle the given create event. </summary>
		/// <param name="event">The save event to be handled. </param>
		/// <param name="createCache"></param>
		protected virtual void EntityIsTransient(PersistEvent @event, IDictionary createCache)
		{
			log.Debug("saving transient instance");

			IEventSource source = @event.Session;
			object entity = source.PersistenceContext.Unproxy(@event.Entity);

			object tempObject;
			tempObject = createCache[entity];
			createCache[entity] = entity;
			if (tempObject == null)
			{
				SaveWithGeneratedId(entity, @event.EntityName, createCache, source, false);
			}
		}
	}
}
