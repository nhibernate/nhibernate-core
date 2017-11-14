using System;
using System.Diagnostics;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Persister.Entity;

namespace NHibernate.Action
{
	[Serializable]
	public sealed partial class EntityIdentityInsertAction : EntityAction
	{
		private readonly object lockObject = new object();
		private readonly object[] state;
		private readonly bool isDelayed;
		private readonly EntityKey delayedEntityKey;
		//private CacheEntry cacheEntry;
		private object generatedId;

		public EntityIdentityInsertAction(object[] state, object instance, IEntityPersister persister, ISessionImplementor session, bool isDelayed)
			: base(session, null, instance, persister)
		{
			this.state = state;
			this.isDelayed = isDelayed;
			delayedEntityKey = this.isDelayed ? GenerateDelayedEntityKey() : null;
		}

		public object GeneratedId
		{
			get { return generatedId; }
		}

		public EntityKey DelayedEntityKey
		{
			get { return delayedEntityKey; }
		}

		private EntityKey GenerateDelayedEntityKey()
		{
			lock (lockObject)
			{
				if (!isDelayed)
					throw new HibernateException("Cannot request delayed entity-key for non-delayed post-insert-id generation");

				return Session.GenerateEntityKey(new DelayedPostInsertIdentifier(), Persister);
			}
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

			bool statsEnabled = Session.Factory.Statistics.IsStatisticsEnabled;
			Stopwatch stopwatch = null;
			if (statsEnabled)
			{
				stopwatch = Stopwatch.StartNew();
			}

			bool veto = PreInsert();

			// Don't need to lock the cache here, since if someone
			// else inserted the same pk first, the insert would fail

			if (!veto)
			{
				generatedId = persister.Insert(state, instance, Session);
				if (persister.HasInsertGeneratedProperties)
				{
					persister.ProcessInsertGeneratedProperties(generatedId, instance, state, Session);
				}
				//need to do that here rather than in the save event listener to let
				//the post insert events to have a id-filled entity when IDENTITY is used (EJB3)
				persister.SetIdentifier(instance, generatedId);
			}

			//TODO from H3.2 : this bit actually has to be called after all cascades!
			//      but since identity insert is called *synchronously*,
			//      instead of asynchronously as other actions, it isn't
			/*if ( persister.hasCache() && !persister.isCacheInvalidationRequired() ) {
			cacheEntry = new CacheEntry(object, persister, session);
			persister.getCache().insert(generatedId, cacheEntry);
			}*/

			PostInsert();
			if (statsEnabled && !veto)
			{
				stopwatch.Stop();
				Session.Factory.StatisticsImplementor.InsertEntity(Persister.EntityName, stopwatch.Elapsed);
			}
		}

		private void PostInsert()
		{
			if (isDelayed)
			{
				Session.PersistenceContext.ReplaceDelayedEntityIdentityInsertKeys(delayedEntityKey, generatedId);
			}
			IPostInsertEventListener[] postListeners = Session.Listeners.PostInsertEventListeners;
			if (postListeners.Length > 0)
			{
				PostInsertEvent postEvent = new PostInsertEvent(Instance, generatedId, state, Persister, (IEventSource)Session);
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
				var postEvent = new PostInsertEvent(Instance, generatedId, state, Persister, (IEventSource) Session);
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
				var preEvent = new PreInsertEvent(Instance, null, state, Persister, (IEventSource) Session);
				foreach (IPreInsertEventListener listener in preListeners)
				{
					veto |= listener.OnPreInsert(preEvent);
				}
			}
			return veto;
		}

		protected override void AfterTransactionCompletionProcessImpl(bool success)
		{
			//TODO Make 100% certain that this is called before any subsequent ScheduledUpdate.afterTransactionCompletion()!!
			//TODO from H3.2: reenable if we also fix the above todo
			/*EntityPersister persister = getEntityPersister();
			if ( success && persister.hasCache() && !persister.isCacheInvalidationRequired() ) {
			persister.getCache().afterInsert( getGeneratedId(), cacheEntry );
			}*/
			if (success)
			{
				PostCommitInsert();
			}
		}
	}
}
