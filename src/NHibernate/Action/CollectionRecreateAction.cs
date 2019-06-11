using System;
using System.Diagnostics;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Persister.Collection;

namespace NHibernate.Action
{
	[Serializable]
	public sealed partial class CollectionRecreateAction : CollectionAction
	{
		public CollectionRecreateAction(IPersistentCollection collection, ICollectionPersister persister, object key, ISessionImplementor session)
			: base(persister, collection, key, session) { }

		/// <summary> Execute this action</summary>
		/// <remarks>
		/// This method is called when a new non-null collection is persisted
		/// or when an existing (non-null) collection is moved to a new owner
		/// </remarks>
		public override void Execute()
		{
			bool statsEnabled = Session.Factory.Statistics.IsStatisticsEnabled;
			Stopwatch stopwatch = null;
			if (statsEnabled)
			{
				stopwatch = Stopwatch.StartNew();
			}
			IPersistentCollection collection = Collection;

			PreRecreate();

			var key = GetKey();
			Persister.Recreate(collection, key, Session);

			var entry = Session.PersistenceContext.GetCollectionEntry(collection);
			// On collection create, the current key may refer a delayed post insert instance instead
			// of the actual identifier, that GetKey above should have resolved at that point. Update
			// it.
			entry.CurrentKey = key;
			entry.AfterAction(collection);

			Evict();

			PostRecreate();
			if (statsEnabled)
			{
				stopwatch.Stop();
				Session.Factory.StatisticsImplementor.RecreateCollection(Persister.Role, stopwatch.Elapsed);
			}
		}

		private void PreRecreate()
		{
			IPreCollectionRecreateEventListener[] preListeners = Session.Listeners.PreCollectionRecreateEventListeners;
			if (preListeners.Length > 0)
			{
				PreCollectionRecreateEvent preEvent = new PreCollectionRecreateEvent(Persister, Collection, (IEventSource)Session);
				for (int i = 0; i < preListeners.Length; i++)
				{
					preListeners[i].OnPreRecreateCollection(preEvent);
				}
			}
		}

		public override void ExecuteAfterTransactionCompletion(bool success)
		{
			var cacheKey = new CacheKey(GetKey(), Persister.KeyType, Persister.Role, Session.Factory);
			
			base.ExecuteAfterTransactionCompletion(success);
			if (success)
			{
				if (Collection.WasInitialized && Session.PersistenceContext.ContainsCollection(Collection))
				{
					CollectionCacheEntry entry = CollectionCacheEntry.Create(Collection, Persister);
					bool put = Persister.Cache.Put(
						cacheKey,
						Persister.CacheEntryStructure.Structure(entry),
						Session.Timestamp + Persister.Cache.Cache.NextTimestamp(),
						null,
						Persister.OwnerEntityPersister.VersionType.Comparator,
						Session.Factory.Settings.IsMinimalPutsEnabled &&
						Session.CacheMode != CacheMode.Refresh);

					if (put && Session.Factory.Statistics.IsStatisticsEnabled)
					{
						Session.Factory.StatisticsImplementor.SecondLevelCachePut(Persister.Cache.RegionName);
					}
				}
			}
			else
			{
				Persister.Cache.Release(cacheKey, Lock);
			}
		}

		private void PostRecreate()
		{
			IPostCollectionRecreateEventListener[] postListeners = Session.Listeners.PostCollectionRecreateEventListeners;
			if (postListeners.Length > 0)
			{
				PostCollectionRecreateEvent postEvent = new PostCollectionRecreateEvent(Persister, Collection, (IEventSource)Session);
				for (int i = 0; i < postListeners.Length; i++)
				{
					postListeners[i].OnPostRecreateCollection(postEvent);
				}
			}
		}
	}
}
