using System;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Collection;

namespace NHibernate.Action
{
	[Serializable]
	public sealed class CollectionUpdateAction : CollectionAction 
	{
		private readonly bool emptySnapshot;

		public CollectionUpdateAction(IPersistentCollection collection, ICollectionPersister persister,
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

			// TODO: H3.2 not ported
			//if (Session.Factory.Statistics.StatisticsEnabled)
			//{
			//  Session.Factory.StatisticsImplementor.updateCollection(Persister.Role);
			//}
		}

		public override int CompareTo(CollectionAction other)
		{
			return 0;
		}
	}
}
