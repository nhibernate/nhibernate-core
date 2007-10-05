using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Action
{
	[Serializable]
	public sealed class EntityUpdateAction : EntityAction
	{
		private readonly object[] state;
		private readonly object[] previousState;
		private object previousVersion;
		private object nextVersion;
		private readonly int[] dirtyFields;
		private readonly bool hasDirtyCollection;
		private object cacheEntry;
		private ISoftLock slock;

		public EntityUpdateAction(object id, object[] state, 
			int[] dirtyProperties, bool hasDirtyCollection, 
			object[] previousState, object previousVersion, object nextVersion, object instance, 
			IEntityPersister persister, ISessionImplementor session)
			: base(session, id, instance, persister)
		{
			this.state = state;
			this.previousState = previousState;
			this.previousVersion = previousVersion;
			this.nextVersion = nextVersion;
			dirtyFields = dirtyProperties;
			this.hasDirtyCollection = hasDirtyCollection;
		}

		protected internal override bool HasPostCommitEventListeners
		{
			get { return Session.Listeners.PostCommitUpdateEventListeners.Length > 0; }
		}

		public override void Execute()
		{
			object id = Id;
			IEntityPersister persister = Persister;
			object instance = Instance;

			bool veto = PreUpdate();

			if (persister.IsVersionPropertyGenerated)
			{
				// we need to grab the version value from the entity, otherwise
				// we have issues with generated-version entities that may have
				// multiple actions queued during the same flush
				previousVersion = persister.GetVersion(instance);
			}

			CacheKey ck = null;
			if (persister.HasCache)
			{
				ck = new CacheKey(id, persister.IdentifierType, persister.RootEntityName, Session.Factory);
				slock = persister.Cache.Lock(ck, previousVersion);
			}

			if (!veto)
			{
				persister.Update(id, state, dirtyFields, hasDirtyCollection, previousState, previousVersion, instance, Session);
			}

			EntityEntry entry = Session.PersistenceContext.GetEntry(instance);
			if (entry == null)
			{
				throw new AssertionFailure("Possible nonthreadsafe access to session");
			}

			if (entry.Status == Impl.Status.Loaded || persister.IsVersionPropertyGenerated)
			{
				// get the updated snapshot of the entity state by cloning current state;
				// it is safe to copy in place, since by this time no-one else (should have)
				// has a reference  to the array
				TypeFactory.DeepCopy(state, persister.PropertyTypes, persister.PropertyCheckability, state);
				if (persister.HasUpdateGeneratedProperties)
				{
					// this entity defines proeprty generation, so process those generated
					// values...
					persister.ProcessUpdateGeneratedProperties(id, instance, state, Session);
					if (persister.IsVersionPropertyGenerated)
					{
						nextVersion = Versioning.GetVersion(state, persister);
					}
				}
				// have the entity entry perform post-update processing, passing it the
				// update state and the new version (if one).
				entry.PostUpdate(instance, state, nextVersion);
			}

			if (persister.HasCache)
			{
				if (persister.IsCacheInvalidationRequired || entry.Status != Impl.Status.Loaded)
				{
					persister.Cache.Evict(ck);
				}
				else
				{
					// TODO H3.2 Different behaviour
					//CacheEntry ce = new CacheEntry(state, persister, persister.HasUninitializedLazyProperties(instance, session.EntityMode), nextVersion, Session, instance);
					//cacheEntry = persister.CacheEntryStructure.structure(ce);
					//persister.Cache.Update(ck, cacheEntry, nextVersion, previousVersion);
					cacheEntry = new CacheEntry(instance, persister, Session);
					persister.Cache.Update(ck, cacheEntry);

					// TODO: H3.2 not ported
					//if (put && factory.Statistics.StatisticsEnabled)
					//{
					//  factory.StatisticsImplementor.secondLevelCachePut(Persister.Cache.RegionName);
					//}
				}
			}

			PostUpdate();

			// TODO: H3.2 not ported
			//if (factory.Statistics.StatisticsEnabled && !veto)
			//{
			//  factory.StatisticsImplementor.updateEntity(Persister.EntityName);
			//}
		}

		public override void AfterTransactionCompletion(bool success)
		{
			IEntityPersister persister = Persister;
			if (persister.HasCache)
			{
				CacheKey ck = new CacheKey(Id, persister.IdentifierType, persister.RootEntityName, Session.Factory);

				if (success && cacheEntry != null)
				{
					persister.Cache.AfterUpdate(ck, cacheEntry, nextVersion, slock);

					// TODO H3.2 Different behaviour
					//if (put && Session.Factory.Statistics.StatisticsEnabled)
					//{
					//  Session.Factory.StatisticsImplementor.secondLevelCachePut(Persister.Cache.RegionName);
					//}
				}
				else
				{
					persister.Cache.Release(ck, slock);
				}
			}
			PostCommitUpdate();
		}

		private void PostUpdate()
		{
			IPostUpdateEventListener[] postListeners = Session.Listeners.PostUpdateEventListeners;
			if (postListeners.Length > 0)
			{
				PostUpdateEvent postEvent = new PostUpdateEvent(Instance, Id, state, previousState, Persister, (IEventSource)Session);
				foreach (IPostUpdateEventListener listener in postListeners)
				{
					listener.OnPostUpdate(postEvent);
				}
			}
		}

		private void PostCommitUpdate()
		{
			IPostUpdateEventListener[] postListeners = Session.Listeners.PostCommitUpdateEventListeners;
			if (postListeners.Length > 0)
			{
				PostUpdateEvent postEvent = new PostUpdateEvent(Instance, Id, state, previousState, Persister, (IEventSource)Session);
				foreach (IPostUpdateEventListener listener in postListeners)
				{
					listener.OnPostUpdate(postEvent);
				}
			}
		}

		private bool PreUpdate()
		{
			IPreUpdateEventListener[] preListeners = Session.Listeners.PreUpdateEventListeners;
			bool veto = false;
			if (preListeners.Length > 0)
			{
				PreUpdateEvent preEvent = new PreUpdateEvent(Instance, Id, state, previousState, Persister, Session);
				foreach (IPreUpdateEventListener listener in preListeners)
				{
					veto |= listener.Equals(preEvent);
				}
			}
			return veto;
		}

		public override int CompareTo(EntityAction other)
		{
			return 0;
		}
	}
}
