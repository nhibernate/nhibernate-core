using System;

using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;

namespace NHibernate.Event.Default
{
	/// <summary> 
	/// Defines the default evict event listener used by hibernate for evicting entities
	/// in response to generated flush events.  In particular, this implementation will
	/// remove any hard references to the entity that are held by the infrastructure
	/// (references held by application or other persistent instances are okay) 
	/// </summary>
	[Serializable]
	public class DefaultEvictEventListener : IEvictEventListener
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(DefaultEvictEventListener));

		public virtual void OnEvict(EvictEvent @event)
		{
			IEventSource source = @event.Session;
			object obj = @event.Entity;
			IPersistenceContext persistenceContext = source.PersistenceContext;

			if (obj.IsProxy())
			{
				ILazyInitializer li = ((INHibernateProxy)obj).HibernateLazyInitializer;
				object id = li.Identifier;
				IEntityPersister persister = source.Factory.GetEntityPersister(li.EntityName);
				if (id == null)
				{
					throw new ArgumentException("null identifier");
				}
				EntityKey key = source.GenerateEntityKey(id, persister);
				persistenceContext.RemoveProxy(key);
				if (!li.IsUninitialized)
				{
					object entity = persistenceContext.RemoveEntity(key);
					if (entity != null)
					{
						EntityEntry e = @event.Session.PersistenceContext.RemoveEntry(entity);
						DoEvict(entity, key, e.Persister, @event.Session);
					}
				}
				li.UnsetSession();
			}
			else
			{
				EntityEntry e = persistenceContext.RemoveEntry(obj);
				if (e != null)
				{
					persistenceContext.RemoveEntity(e.EntityKey);
					DoEvict(obj, e.EntityKey, e.Persister, source);
				}
			}
		}

		protected virtual void DoEvict(object obj, EntityKey key, IEntityPersister persister, IEventSource session)
		{

			if (log.IsDebugEnabled)
			{
				log.Debug("evicting " + MessageHelper.InfoString(persister));
			}

			// remove all collections for the entity from the session-level cache
			if (persister.HasCollections)
			{
				new EvictVisitor(session).Process(obj, persister);
			}

			new Cascade(CascadingAction.Evict, CascadePoint.AfterEvict, session).CascadeOn(persister, obj);
		}
	}
}
