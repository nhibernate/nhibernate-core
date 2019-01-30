using System.Collections.Generic;
using NHibernate.Persister.Collection;

namespace NHibernate.Collection.Trackers
{
	/// <summary>
	/// A tracker that is able to track changes that are done to an uninitialized map.
	/// </summary>
	internal class SetQueueOperationTracker<T> : CollectionQueueOperationTracker<T, ISet<T>>
	{
		public SetQueueOperationTracker(ICollectionPersister collectionPersister) : base(collectionPersister)
		{
		}

		protected override bool Add(T element)
		{
			if (!Queue.Add(element))
			{
				return false;
			}

			if (!Cleared)
			{
				QueueSize++;
			}

			return true;
		}
	}
}
