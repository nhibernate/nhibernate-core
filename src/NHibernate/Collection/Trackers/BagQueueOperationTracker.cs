using System.Collections.Generic;
using System.Linq;
using NHibernate.Persister.Collection;

namespace NHibernate.Collection.Trackers
{
	/// <inheritdoc />
	internal class BagQueueOperationTracker<T> : CollectionQueueOperationTracker<T, IList<T>>
	{
		public BagQueueOperationTracker(ICollectionPersister collectionPersister) : base(collectionPersister)
		{
		}

		/// <inheritdoc />
		public override void ApplyChanges(IList<T> loadedCollection)
		{
			if (Cleared)
			{
				loadedCollection.Clear();
			}
			else if (RemovalQueue != null)
			{
				foreach (var toRemove in RemovalQueue)
				{
					loadedCollection.Remove(toRemove);
				}
			}

			if (Queue != null)
			{
				foreach (var toAdd in Queue)
				{
					// NH Different behavior for NH-739. A "bag" mapped as a bidirectional one-to-many of an entity with an
					// id generator causing it to be inserted on flush must not replay the addition after initialization,
					// if the entity was previously saved. In that case, the entity save has inserted it in database with
					// its association to the bag, without causing a full flush. So for the bag, the operation is still
					// pending, but in database it is already done. On initialization, the bag thus already receives the
					// entity in its loaded list, and it should not be added again.
					// Since a one-to-many bag is actually a set, we can simply check if the entity is already in the loaded
					// state, and discard it if yes. (It also relies on the bag not having pending removes, which is the
					// case, since it only handles delayed additions and clears.)
					// Since this condition happens with transient instances added in the bag then saved, ReferenceEquals
					// is enough to match them.
					// This solution is a workaround, the root cause is not fixed. The root cause is the insertion on save
					// done without caring for pending operations of one-to-many collections. This root cause could be fixed
					// by triggering a full flush there before the insert (currently it just flushes pending inserts), or
					// maybe by flushing just the dirty one-to-many non-initialized collections (but this can be tricky).
					// (It is also likely one-to-many lists have a similar issue, but nothing is done yet for them. And
					// their case is more complex due to having to care for the indexes and to handle many more delayed
					// operation kinds.)
					if (CollectionPersister.IsOneToMany && loadedCollection.Any(l => ReferenceEquals(l, toAdd)))
					{
						continue;
					}

					loadedCollection.Add(toAdd);
				}
			}
		}
	}
}
