using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Impl;
using NHibernate.Persister.Entity;

namespace NHibernate.Action
{
	[Serializable]
	public sealed class EntityDeleteAction : EntityAction
	{
		private readonly object[] state;
		private readonly object version;
		private readonly bool isCascadeDeleteEnabled;
		private ISoftLock sLock;

		public EntityDeleteAction(object id, object[] state, object version, object instance, IEntityPersister persister, bool isCascadeDeleteEnabled, ISessionImplementor session)
			: base(session, id, instance, persister)
		{
			this.state = state;
			this.version = version;
			this.isCascadeDeleteEnabled = isCascadeDeleteEnabled;
		}

		protected internal override bool HasPostCommitEventListeners
		{
			get { return Session.Listeners.PostDeleteEventListeners.Length > 0; }
		}

		public override void Execute()
		{
			object id = Id;
			IEntityPersister persister = Persister;
			ISessionImplementor session = Session;
			object instance = Instance;

			bool veto = PreDelete();

			object tmpVersion = version;
			if (persister.IsVersionPropertyGenerated)
			{
				// we need to grab the version value from the entity, otherwise
				// we have issues with generated-version entities that may have
				// multiple actions queued during the same flush
				tmpVersion = persister.GetVersion(instance);
			}

			CacheKey ck;
			if (persister.HasCache)
			{
				ck = new CacheKey(id, persister.IdentifierType, persister.RootEntityName, session.Factory);
				sLock = persister.Cache.Lock(ck, version);
			}
			else
			{
				ck = null;
			}

			if (!isCascadeDeleteEnabled && !veto)
			{
				persister.Delete(id, tmpVersion, instance, session);
			}

			//postDelete:
			// After actually deleting a row, record the fact that the instance no longer 
			// exists on the database (needed for identity-column key generation), and
			// remove it from the session cache
			IPersistenceContext persistenceContext = session.PersistenceContext;

			EntityEntry entry = persistenceContext.RemoveEntry(instance);
			if (entry == null)
			{
				throw new AssertionFailure("Possible nonthreadsafe access to session");
			}
			entry.PostDelete();

			EntityKey key = new EntityKey(entry.Id, entry.Persister);
			persistenceContext.RemoveEntity(key);
			persistenceContext.RemoveProxy(key);

			if (persister.HasCache)
				persister.Cache.Evict(ck);

			PostDelete();

			// TODO: H3.2 not ported
			//if (Session.Factory.Statistics.StatisticsEnabled && !veto)
			//{
			//  Session.Factory.StatisticsImplementor.deleteEntity(Persister.EntityName);
			//}
		}

		private void PostDelete()
		{
			IPostDeleteEventListener[] postListeners = Session.Listeners.PostDeleteEventListeners;
			if (postListeners.Length > 0)
			{
				PostDeleteEvent postEvent = new PostDeleteEvent(Instance, Id, state, Persister, (IEventSource)Session);
				foreach (IPostDeleteEventListener listener in postListeners)
				{
					listener.OnPostDelete(postEvent);
				}
			}
		}

		private bool PreDelete()
		{
			IPreDeleteEventListener[] preListeners = Session.Listeners.PreDeleteEventListeners;
			bool veto = false;
			if (preListeners.Length > 0)
			{
				PreDeleteEvent preEvent = new PreDeleteEvent(Instance, Id, state, Persister);
				foreach (IPreDeleteEventListener listener in preListeners)
				{
					veto |= listener.OnPreDelete(preEvent);
				}
			}
			return veto;
		}

		public override void AfterTransactionCompletion(bool success)
		{
			if (Persister.HasCache)
			{
				CacheKey ck = new CacheKey(Id, Persister.IdentifierType, Persister.RootEntityName, Session.Factory);
				Persister.Cache.Release(ck, sLock);
			}
			PostCommitDelete();
		}

		private void PostCommitDelete()
		{
			IPostDeleteEventListener[] postListeners = Session.Listeners.PostCommitDeleteEventListeners;
			if (postListeners.Length > 0)
			{
				PostDeleteEvent postEvent = new PostDeleteEvent(Instance, Id, state, Persister, (IEventSource)Session);
				foreach (IPostDeleteEventListener listener in postListeners)
				{
					listener.OnPostDelete(postEvent);
				}
			}
		}

		public override int CompareTo(EntityAction other)
		{
			return 0;
		}
	}
}
