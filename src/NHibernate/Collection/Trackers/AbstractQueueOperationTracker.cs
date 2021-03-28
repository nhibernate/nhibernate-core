using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Collection.Trackers
{
	/// <summary>
	/// A tracker that is able to track changes that are done to an uninitialized collection.
	/// </summary>
	internal abstract class AbstractQueueOperationTracker
	{
		internal static HashSet<string> IndexOperations = new HashSet<string>
		{
			nameof(AbstractCollectionQueueOperationTracker<int>.RemoveElementAtIndex),
			nameof(AbstractCollectionQueueOperationTracker<int>.AddElementAtIndex),
			nameof(AbstractCollectionQueueOperationTracker<int>.SetElementAtIndex),
			nameof(AbstractCollectionQueueOperationTracker<int>.TryGetElementAtIndex)
		};

		/// <summary>
		/// The number of elements that the collection have in the database.
		/// </summary>
		public virtual int? DatabaseCollectionSize { get; protected internal set; }

		/// <summary>
		/// Whether the Clear operation was performed on the uninitialized collection.
		/// </summary>
		public virtual bool Cleared { get; protected set; }

		/// <summary>
		/// Returns the current size of the queue that can be negative when there are more removed than added elements.
		/// </summary>
		/// <returns>The queue size.</returns>
		public abstract int GetQueueSize();

		/// <summary>
		/// Returns the current size of the collection by taking into the consideration the queued operations.
		/// </summary>
		/// <returns>The current collection size.</returns>
		public int GetCollectionSize()
		{
			if (Cleared)
			{
				return GetQueueSize();
			}

			if (!DatabaseCollectionSize.HasValue)
			{
				throw new InvalidOperationException($"{nameof(DatabaseCollectionSize)} is not set");
			}

			return DatabaseCollectionSize.Value + GetQueueSize();
		}

		/// <summary>
		/// Checks whether the database collection size is required for the given operation.
		/// </summary>
		/// <param name="operationName">The operation name to check.</param>
		/// <returns>True whether the database collection size is required, false otherwise.</returns>
		public virtual bool RequiresDatabaseCollectionSize(string operationName) => false;

		/// <summary>
		/// Checks whether flushing is required for the given operation.
		/// </summary>
		/// <param name="operationName">The operation name to check.</param>
		/// <returns>True whether flushing is required, false otherwise.</returns>
		public virtual bool RequiresFlushing(string operationName) => false;

		/// <summary>
		/// A method that will be called once the flushing is done.
		/// </summary>
		public abstract void AfterFlushing();

		/// <summary>
		/// A method that will be called before an operation.
		/// </summary>
		/// <param name="operationName">The operation that will be executed.</param>
		public virtual void BeforeOperation(string operationName) { }

		/// <summary>
		/// A method that will be called when a Clear operation is performed on the collection.
		/// </summary>
		public virtual void ClearCollection()
		{
			Cleared = true;
		}

		/// <summary>
		/// Returns an <see cref="IEnumerable"/> of elements that were added into the collection.
		/// </summary>
		/// <returns>An <see cref="IEnumerable"/> of added elements.</returns>
		public abstract IEnumerable GetAddedElements();

		/// <summary>
		/// Returns an <see cref="IEnumerable"/> of orphan elements of the collection.
		/// </summary>
		/// <returns>An <see cref="IEnumerable"/> of orphan elements.</returns>
		public abstract IEnumerable GetOrphans();

		/// <summary>
		/// Checks whether a write operation was performed.
		/// </summary>
		/// <returns>True whether a write operation was performed, false otherwise.</returns>
		public abstract bool HasChanges();
	}
}
