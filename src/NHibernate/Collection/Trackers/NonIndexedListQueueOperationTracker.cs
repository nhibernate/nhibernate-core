using System.Collections.Generic;
using NHibernate.Persister.Collection;

namespace NHibernate.Collection.Trackers
{
	/// <inheritdoc />
	internal class NonIndexedListQueueOperationTracker<T> : CollectionQueueOperationTracker<T, IList<T>>
	{
		public NonIndexedListQueueOperationTracker(ICollectionPersister collectionPersister) : base(collectionPersister)
		{
		}
	}
}
