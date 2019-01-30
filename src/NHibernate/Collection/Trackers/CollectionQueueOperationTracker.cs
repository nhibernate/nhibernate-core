using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Persister.Collection;

namespace NHibernate.Collection.Trackers
{
	internal abstract class CollectionQueueOperationTracker<T, TCollection> : AbstractCollectionQueueOperationTracker<T, TCollection>
		where TCollection : ICollection<T>
	{
		protected TCollection Queue;
		protected ISet<T> RemovalQueue;
		protected int QueueSize;
		protected ISet<T> Orphans;
		protected readonly ICollectionPersister CollectionPersister;

		protected CollectionQueueOperationTracker(ICollectionPersister collectionPersister)
		{
			CollectionPersister = collectionPersister;
		}

		/// <inheritdoc />
		public override bool AddElement(T element)
		{
			InitializeQueue();
			RemovalQueue?.Remove(element); // We have to remove the element from the removal list when the element is readded
			return Add(element);
		}

		protected virtual bool Add(T element)
		{
			Queue.Add(element);
			if (!Cleared)
			{
				QueueSize++;
			}

			return true;
		}

		/// <inheritdoc />
		public override void AfterFlushing()
		{
			Queue?.Clear();
			QueueSize = 0;
			Orphans?.Clear();
			RemovalQueue?.Clear();
		}

		/// <inheritdoc />
		public override bool HasChanges()
		{
			return Cleared || RemovalQueue?.Count > 0 || Queue?.Count > 0;
		}

		/// <inheritdoc />
		public override void ApplyChanges(TCollection loadedCollection)
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
					loadedCollection.Add(toAdd);
				}
			}
		}

		/// <inheritdoc />
		public override bool RequiresFlushing(string operationName)
		{
			return IndexOperations.Contains(operationName);
		}

		/// <inheritdoc />
		public override void RemoveExistingElement(T element, bool? existsInDb)
		{
			if (!Cleared)
			{
				// As this method is called only when the element exists in the queue or in database, we can safely reduce the queue size
				QueueSize--;
			}

			if (existsInDb == true)
			{
				GetOrCreateOrphansSet().Add(element);
			}

			GetOrCreateRemovalQueue().Add(element);
			Queue?.Remove(element);
		}

		/// <inheritdoc />
		public override void ClearCollection()
		{
			AfterFlushing();
			Cleared = true;
		}

		/// <inheritdoc />
		public override bool ContainsElement(T element)
		{
			return Queue?.Contains(element) ?? false;
		}

		/// <inheritdoc />
		public override int GetQueueSize()
		{
			return Cleared ? (Queue?.Count ?? 0) : QueueSize;
		}

		/// <inheritdoc />
		public override bool IsElementQueuedForDelete(T element)
		{
			return RemovalQueue?.Contains(element) ?? false;
		}

		/// <inheritdoc />
		public override IEnumerable GetAddedElements()
		{
			return (IEnumerable) Queue ?? Enumerable.Empty<object>();
		}

		/// <inheritdoc />
		public override IEnumerable GetOrphans()
		{
			return (IEnumerable) Orphans ?? Enumerable.Empty<object>();
		}

		/// <inheritdoc />
		public override void RemoveElementAtIndex(int index, T element)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override void AddElementAtIndex(int index, T element)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override void SetElementAtIndex(int index, T element, T oldElement)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override bool TryGetElementAtIndex(int index, out T element)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override int CalculateDatabaseElementIndex(int index)
		{
			throw new NotSupportedException();
		}

		protected ISet<T> GetOrCreateRemovalQueue()
		{
			return RemovalQueue ?? (RemovalQueue = new HashSet<T>());
		}

		protected ISet<T> GetOrCreateOrphansSet()
		{
			return Orphans ?? (Orphans = new HashSet<T>());
		}

		private void InitializeQueue()
		{
			if (Queue == null)
			{
				Queue = (TCollection) CollectionPersister.CollectionType.Instantiate(-1);
			}
		}
	}
}
