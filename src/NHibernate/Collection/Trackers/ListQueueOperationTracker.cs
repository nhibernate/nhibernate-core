﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Persister.Collection;

namespace NHibernate.Collection.Trackers
{
	/// <inheritdoc />
	internal class ListQueueOperationTracker<T> : AbstractCollectionQueueOperationTracker<T, IList<T>>
	{
		private readonly ICollectionPersister _collectionPersister;
		private AbstractCollectionQueueOperationTracker<T, IList<T>> _tracker;
		private ISet<T> _flushedElements;

		public ListQueueOperationTracker(ICollectionPersister collectionPersister)
		{
			_collectionPersister = collectionPersister;
		}

		/// <inheritdoc />
		public override int DatabaseCollectionSize
		{
			get => _tracker?.DatabaseCollectionSize ?? base.DatabaseCollectionSize;
			protected internal set
			{
				base.DatabaseCollectionSize = value;
				if (_tracker != null)
				{
					_tracker.DatabaseCollectionSize = value;
				}
			}
		}

		/// <inheritdoc />
		public override bool RequiresDatabaseCollectionSize(string operationName)
		{
			return _tracker?.RequiresDatabaseCollectionSize(operationName) ?? IndexOperations.Contains(operationName);
		}

		/// <inheritdoc />
		public override int GetQueueSize()
		{
			return _tracker?.GetQueueSize() ?? 0;
		}

		/// <inheritdoc />
		public override void ClearCollection()
		{
			// ClearedListQueueOperationTracker does not need the flushed elements as it will never
			// use the database collection size to calculate the current size
			_flushedElements = null;
			_tracker = new ClearedListQueueOperationTracker<T>(_collectionPersister);
			Cleared = true;
		}

		/// <inheritdoc />
		public override IEnumerable GetAddedElements()
		{
			return _tracker?.GetAddedElements() ?? Enumerable.Empty<object>();
		}

		/// <inheritdoc />
		public override IEnumerable GetOrphans()
		{
			return _tracker?.GetOrphans() ?? Enumerable.Empty<object>();
		}

		/// <inheritdoc />
		protected internal override void AfterElementFlushing(T element)
		{
			if (_tracker == null)
			{
				GetOrCreateFlushedElements().Add(element);
				return;
			}

			_tracker.AfterElementFlushing(element);
		}

		/// <inheritdoc />
		public override bool RequiresFlushing(string operationName)
		{
			return _tracker?.RequiresFlushing(operationName) == true;
		}

		/// <inheritdoc />
		public override void AfterFlushing()
		{
			// We have to reset the current database collection size in case an element
			// was added multiple times
			DatabaseCollectionSize = -1;
			_tracker = null;
		}

		public override void BeforeOperation(string operationName)
		{
			if (_tracker != null)
			{
				return;
			}

			_tracker = IndexOperations.Contains(operationName)
				? (AbstractCollectionQueueOperationTracker<T, IList<T>>) new IndexedListQueueOperationTracker<T>(_flushedElements)
				{
					DatabaseCollectionSize = DatabaseCollectionSize
				}
				: new NonIndexedListQueueOperationTracker<T>(_collectionPersister)
				{
					DatabaseCollectionSize = DatabaseCollectionSize,
					FlushedElements = _flushedElements
				};
		}

		/// <inheritdoc />
		public override bool AddElement(T element)
		{
			return GetOrCreateStrategy().AddElement(element);
		}

		/// <inheritdoc />
		public override void RemoveExistingElement(T element, bool? existsInDb)
		{
			GetOrCreateStrategy().RemoveExistingElement(element, existsInDb);
		}

		/// <inheritdoc />
		public override bool ContainsElement(T element)
		{
			return _tracker?.ContainsElement(element) == true;
		}

		/// <inheritdoc />
		public override bool IsElementQueuedForDelete(T element)
		{
			return _tracker?.IsElementQueuedForDelete(element) == true;
		}

		/// <inheritdoc />
		public override void RemoveElementAtIndex(int index, T element)
		{
			GetOrCreateIndexedStrategy().RemoveElementAtIndex(index, element);
		}

		/// <inheritdoc />
		public override void AddElementAtIndex(int index, T element)
		{
			GetOrCreateIndexedStrategy().AddElementAtIndex(index, element);
		}

		/// <inheritdoc />
		public override void SetElementAtIndex(int index, T element, T oldElement)
		{
			GetOrCreateIndexedStrategy().SetElementAtIndex(index, element, oldElement);
		}

		/// <inheritdoc />
		public override bool TryGetElementAtIndex(int index, out T element)
		{
			if (_tracker != null)
			{
				return _tracker.TryGetElementAtIndex(index, out element);
			}

			element = default(T);
			return false;
		}

		/// <inheritdoc />
		public override int CalculateDatabaseElementIndex(int index)
		{
			return _tracker?.CalculateDatabaseElementIndex(index) ?? index;
		}

		/// <inheritdoc />
		public override void ApplyChanges(IList<T> loadedCollection)
		{
			_tracker?.ApplyChanges(loadedCollection);
		}

		private AbstractCollectionQueueOperationTracker<T, IList<T>> GetOrCreateStrategy()
		{
			return _tracker ?? (_tracker = new NonIndexedListQueueOperationTracker<T>(_collectionPersister)
			{
				DatabaseCollectionSize = DatabaseCollectionSize,
				FlushedElements = _flushedElements
			});
		}

		private AbstractCollectionQueueOperationTracker<T, IList<T>> GetOrCreateIndexedStrategy()
		{
			return _tracker ?? (_tracker = new IndexedListQueueOperationTracker<T>(_flushedElements)
			{
				DatabaseCollectionSize = DatabaseCollectionSize
			});
		}

		protected ISet<T> GetOrCreateFlushedElements()
		{
			return _flushedElements ?? (_flushedElements = new HashSet<T>());
		}

		public override bool HasChanges()
		{
			return _tracker?.HasChanges() ?? false;
		}
	}
}
