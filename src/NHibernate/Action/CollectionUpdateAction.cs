namespace NHibernate.Action
{
	using System;
	using Cache;
	using Collection;
	using Engine;
	using Impl;
	using NHibernate.Persister.Collection;

	[Serializable]
	public sealed class CollectionUpdateAction : CollectionAction
	{
		private readonly bool emptySnapshot;

		public CollectionUpdateAction(
			IPersistentCollection collection, ICollectionPersister persister,
			object key, bool emptySnapshot, ISessionImplementor session)
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

			if (!collection.WasInitialized)
			{
				if (!collection.HasQueuedAdds)
					throw new AssertionFailure("no queued adds");
				//do nothing - we only need to notify the cache...
			}
			else if (!affectedByFilters && collection.Empty)
			{
				if (!emptySnapshot)
					persister.Remove(id, session);
			}
			else if (collection.NeedsRecreate(persister))
			{
				if (affectedByFilters)
				{
					throw new HibernateException("cannot recreate collection while filter is enabled: " + MessageHelper.InfoString(persister, id, persister.Factory));
				}
				if (!emptySnapshot)
					persister.Remove(id, session);
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

			if (Session.Factory.Statistics.IsStatisticsEnabled)
			{
				Session.Factory.StatisticsImplementor.UpdateCollection(Persister.Role);
			}
		}

		public override void AfterTransactionCompletion(bool success)
		{
			if (Persister.HasCache)
			{
				CacheKey ck = new CacheKey(Key, Persister.KeyType, Persister.Role, Session.EntityMode, Session.Factory);

				if (success )
				{
					// we can't disassemble a collection if it was uninitialized 
					// or detached from the session
					if (Collection.WasInitialized 
						&& Session.PersistenceContext.ContainsCollection(Collection))
					{
						bool put = Persister.Cache.AfterUpdate(ck, Collection.Disassemble(Persister), null, Lock);

						if (put && Session.Factory.Statistics.IsStatisticsEnabled)
						{
							Session.Factory.StatisticsImplementor.SecondLevelCachePut(Persister.Cache.RegionName);
						}
					}
				}
				else
				{
					Persister.Cache.Release(ck, Lock);
				}
			}
		}
	}
}