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
		public override void AfterFlushing()
		{
			// We have to reset the current database collection size in case an element
			// was added multiple times
			DatabaseCollectionSize = -1;
			base.AfterFlushing();
		}
	}
}
