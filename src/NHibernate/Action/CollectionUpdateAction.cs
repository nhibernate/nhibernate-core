using System;
using System.Diagnostics;
using NHibernate.Cache;
using NHibernate.Cache.Access;
using NHibernate.Cache.Entry;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Impl;
using NHibernate.Persister.Collection;

namespace NHibernate.Action
{
	[Serializable]
	public sealed partial class CollectionUpdateAction : CollectionAction
	{
		private readonly bool emptySnapshot;

		public CollectionUpdateAction(IPersistentCollection collection, ICollectionPersister persister, object key,
									  bool emptySnapshot, ISessionImplementor session)
			: base(persister, collection, key, session)
		{
			this.emptySnapshot = emptySnapshot;
		}

		public override void Execute()
		{
			object id = Key;
			ISessionImplementor session = Session;
			ICollectionPersister persister = Persister;
			IPersistentCollection collection = Collection;
			bool affectedByFilters = persister.IsAffectedByEnabledFilters(session);

			bool statsEnabled = session.Factory.Statistics.IsStatisticsEnabled;
			Stopwatch stopwatch = null;
			if (statsEnabled)
			{
				stopwatch = Stopwatch.StartNew();
			}

			PreUpdate();

			if (!collection.WasInitialized)
			{
				if (!collection.HasQueuedOperations)
				{
					throw new AssertionFailure("no queued adds");
				}
				//do nothing - we only need to notify the cache...
			}
			else if (!affectedByFilters && collection.Empty)
			{
				if (!emptySnapshot)
				{
					persister.Remove(id, session);
				}
			}
			else if (collection.NeedsRecreate(persister))
			{
				if (affectedByFilters)
				{
					throw new HibernateException("cannot recreate collection while filter is enabled: "
												 + MessageHelper.CollectionInfoString(persister, collection, id, session));
				}
				if (!emptySnapshot)
				{
					persister.Remove(id, session);
				}
				persister.Recreate(collection, id, session);
			}
			else
			{
				persister.DeleteRows(collection, id, session);
				persister.UpdateRows(collection, id, session);
				persister.InsertRows(collection, id, session);
			}

			Session.PersistenceContext.GetCollectionEntry(collection).AfterAction(collection);

			Evict();

			PostUpdate();

			if (statsEnabled)
			{
				stopwatch.Stop();
				Session.Factory.StatisticsImplementor.UpdateCollection(Persister.Role, stopwatch.Elapsed);
			}
		}

		private void PreUpdate()
		{
			IPreCollectionUpdateEventListener[] preListeners = Session.Listeners.PreCollectionUpdateEventListeners;
			if (preListeners.Length > 0)
			{
				PreCollectionUpdateEvent preEvent = new PreCollectionUpdateEvent(Persister, Collection, (IEventSource)Session);
				for (int i = 0; i < preListeners.Length; i++)
				{
					preListeners[i].OnPreUpdateCollection(preEvent);
				}
			}
		}

		private void PostUpdate()
		{
			IPostCollectionUpdateEventListener[] postListeners = Session.Listeners.PostCollectionUpdateEventListeners;
			if (postListeners.Length > 0)
			{
				PostCollectionUpdateEvent postEvent = new PostCollectionUpdateEvent(Persister, Collection, (IEventSource)Session);
				for (int i = 0; i < postListeners.Length; i++)
				{
					postListeners[i].OnPostUpdateCollection(postEvent);
				}
			}
		}

		public override IBeforeTransactionCompletionProcess BeforeTransactionCompletionProcess
		{
			get 
			{ 
				return null; 
			}
		}

		public override IAfterTransactionCompletionProcess AfterTransactionCompletionProcess
		{
			get
			{
				// Only make sense to add the delegate if there is a cache.
				if (Persister.HasCache)
				{
					return new CollectionCacheUpdate(this);
				}
				return null;
			}
		}

		private partial class CollectionCacheUpdate : IAfterTransactionCompletionProcess
		{

			private CollectionUpdateAction _action;

			internal CollectionCacheUpdate(CollectionUpdateAction action)
			{
				_action = action;
			}

			public void Execute(bool success)
			{
				var session = _action.Session;
				var persister = _action.Persister;
				var cacheLock = _action.Lock;
				CacheKey ck = session.GenerateCacheKey(_action.Key, persister.KeyType, persister.Role);

				if (success)
				{
					var collection = _action.Collection;
					
					// we can't disassemble a collection if it was uninitialized 
					// or detached from the session
					if (collection.WasInitialized && session.PersistenceContext.ContainsCollection(collection))
					{
						CollectionCacheEntry entry = CollectionCacheEntry.Create(collection, persister);
						bool put = persister.Cache.AfterUpdate(ck, entry, null, cacheLock);

						if (put && session.Factory.Statistics.IsStatisticsEnabled)
						{
							session.Factory.StatisticsImplementor.SecondLevelCachePut(persister.Cache.RegionName);
						}
					}
				}
				else
				{
					persister.Cache.Release(ck, cacheLock);
				}
			}
		}
	}
}
