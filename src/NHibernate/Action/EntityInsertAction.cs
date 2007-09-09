using System;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Impl;
using NHibernate.Persister.Entity;

namespace NHibernate.Action
{
	[Serializable]
	public sealed class EntityInsertAction : EntityAction
	{
		private readonly object[] state;
		private object version;
		private object cacheEntry;

		public EntityInsertAction(object id, object[] state, object instance, object version, IEntityPersister persister, ISessionImplementor session)
			: base(session, id, instance, persister)
		{
			this.state = state;
			this.version = version;
		}

		protected internal override bool HasPostCommitEventListeners
		{
			get
			{
				return Session.Listeners.PostCommitInsertEventListeners.Length > 0;
			}
		}

		public override void Execute()
		{
			IEntityPersister persister = Persister;
			object instance = Instance;
			object id = Id;

			bool veto = PreInsert();

			// Don't need to lock the cache here, since if someone
			// else inserted the same pk first, the insert would fail
			if (!veto)
			{

				persister.Insert(id, state, instance, Session);

				EntityEntry entry = Session.GetEntry(instance);
				if (entry == null)
				{
					throw new HibernateException("Possible nonthreadsafe access to session");
				}

				entry.PostInsert();

				if (persister.HasInsertGeneratedProperties)
				{
					persister.ProcessInsertGeneratedProperties(id, instance, state, Session);
					if (persister.IsVersionPropertyGenerated)
					{
						version = Versioning.GetVersion(state, persister);
					}
					entry.PostUpdate(instance, state, version);
				}
			}

			if (IsCachePutEnabled(persister))
			{
				// TODO H3.2 Different behaviour
				//CacheEntry ce = new CacheEntry(state, persister, persister.HasUninitializedLazyProperties(instance), version, session, instance);
				//cacheEntry = persister.CacheEntryStructure.structure(ce);
				cacheEntry = new CacheEntry(state, persister, Session);

				CacheKey ck = new CacheKey(id, persister.IdentifierType, persister.RootEntityName, Session.Factory);
				persister.Cache.Insert(ck, cacheEntry);

				// TODO: H3.2 not ported
				//if (put && factory.Statistics.StatisticsEnabled)
				//{
				//  factory.StatisticsImplementor.secondLevelCachePut(Persister.Cache.RegionName);
				//}
			}

			PostInsert();

			// TODO: H3.2 not ported
			//if (factory.Statistics.StatisticsEnabled && !veto)
			//{
			//  factory.StatisticsImplementor.insertEntity(Persister.EntityName);
			//}
		}

		public override void AfterTransactionCompletion(bool success)
		{
			//Make 100% certain that this is called before any subsequent ScheduledUpdate.afterTransactionCompletion()!!
			IEntityPersister persister = Persister;
			if (success && IsCachePutEnabled(persister))
			{
				CacheKey ck = new CacheKey(Id, persister.IdentifierType, persister.RootEntityName, Session.Factory);
				persister.Cache.AfterInsert(ck, cacheEntry, version);

				// TODO: H3.2 not ported
				//if (put && Session.Factory.Statistics.StatisticsEnabled)
				//{
				//  Session.Factory.StatisticsImplementor.secondLevelCachePut(Persister.Cache.RegionName);
				//}
			}
			PostCommitInsert();
		}

		private void PostInsert()
		{
			IPostInsertEventListener[] postListeners = Session.Listeners.PostInsertEventListeners;
			if (postListeners.Length > 0)
			{
				PostInsertEvent postEvent = new PostInsertEvent(Instance, Id, state, Persister, (IEventSource)Session);
				foreach (IPostInsertEventListener listener in postListeners)
				{
					listener.OnPostInsert(postEvent);
				}
			}
		}

		private void PostCommitInsert()
		{
			IPostInsertEventListener[] postListeners = Session.Listeners.PostCommitInsertEventListeners;
			if (postListeners.Length > 0)
			{
				PostInsertEvent postEvent = new PostInsertEvent(Instance, Id, state, Persister, (IEventSource)Session);
				foreach (IPostInsertEventListener listener in postListeners)
				{
					listener.OnPostInsert(postEvent);
				}
			}
		}

		private bool PreInsert()
		{
			IPreInsertEventListener[] preListeners = Session.Listeners.PreInsertEventListeners;
			bool veto = false;
			if (preListeners.Length > 0)
			{
				PreInsertEvent preEvent = new PreInsertEvent(Instance, Id, state, Persister, Session);
				foreach (IPreInsertEventListener listener in preListeners)
				{
					veto |= listener.OnPreInsert(preEvent);
				}
			}
			return veto;
		}

		private static bool IsCachePutEnabled(IEntityPersister persister)
		{
			// TODO H3.2 Different behaviour
			//return persister.HasCache && !persister.CacheInvalidationRequired && session.CacheMode.PutEnabled;
			return persister.HasCache && !persister.IsCacheInvalidationRequired;
		}

		public override int CompareTo(EntityAction other)
		{
			return 0;
		}
	}
}
