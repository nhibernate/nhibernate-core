using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Collection.Trackers
{
	/// <inheritdoc />
	internal class IndexedListQueueOperationTracker<T> : AbstractCollectionQueueOperationTracker<T, IList<T>>
	{
		private static readonly KeyValuePairComparer Comparer = new KeyValuePairComparer();
		private class KeyValuePairComparer : IComparer<KeyValuePair<int, T>>
		{
			public int Compare(KeyValuePair<int, T> x, KeyValuePair<int, T> y)
			{
				if (x.Key > y.Key)
				{
					return 1;
				}

				if (x.Key < y.Key)
				{
					return -1;
				}

				return 0;
			}
		}

		private List<KeyValuePair<int, T>> _queue; // Sorted queue by index
		private int _queueSize;
		private List<KeyValuePair<int, T>> _removedDbIndexes; // Sorted removed db indexes
		private ISet<T> _removalQueue;
		private ISet<T> _flushedElements;

		public IndexedListQueueOperationTracker(ISet<T> flushedElements)
		{
			_flushedElements = flushedElements;
		}

		/// <inheritdoc />
		public override bool RequiresFlushing(string operationName)
		{
			// For remove operation we don't know the index of the removal item
			return nameof(RemoveExistingElement) == operationName;
		}

		/// <inheritdoc />
		public override bool RequiresDatabaseCollectionSize(string operationName)
		{
			return IndexOperations.Contains(operationName) || nameof(AddElement) == operationName;
		}

		/// <inheritdoc />
		public override bool ContainsElement(T element)
		{
			if (_queue == null)
			{
				return false;
			}

			foreach (var pair in _queue)
			{
				if (EqualityComparer<T>.Default.Equals(pair.Value, element))
				{
					return true;
				}
			}

			return false;
		}

		/// <inheritdoc />
		public override bool IsElementQueuedForDelete(T element)
		{
			return _removalQueue?.Contains(element) ?? false;
		}

		/// <inheritdoc />
		public override bool HasChanges()
		{
			return _removedDbIndexes?.Count > 0 || _queue?.Count > 0;
		}

		/// <inheritdoc />
		public override void ApplyChanges(IList<T> loadedCollection)
		{
			if (_removedDbIndexes != null)
			{
				for (var i = _removedDbIndexes.Count - 1; i >= 0; i--)
				{
					loadedCollection.RemoveAt(_removedDbIndexes[i].Key);
				}
			}

			if (_queue != null)
			{
				for (var i = 0; i < _queue.Count; i++)
				{
					var pair = _queue[i];
					loadedCollection.Insert(pair.Key, pair.Value);
				}
			}
		}

		/// <inheritdoc />
		public override bool AddElement(T element)
		{
			AddElementAtIndex(GetCollectionSize(), element);
			return true;
		}

		/// <inheritdoc />
		public override void RemoveExistingElement(T element, bool? existsInDb)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override void ClearCollection()
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override void AfterFlushing()
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override IEnumerable GetAddedElements()
		{
			return _queue?.Select(o => o.Value) ?? (IEnumerable) Enumerable.Empty<object>();
		}

		/// <inheritdoc />
		public override IEnumerable GetOrphans()
		{
			return _removedDbIndexes?.Select(o => o.Value) ?? (IEnumerable) Enumerable.Empty<object>();
		}

		/// <inheritdoc />
		public override bool Cleared
		{
			get => false;
			protected set => throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override int GetQueueSize()
		{
			return _queueSize;
		}

		/// <inheritdoc />
		protected internal override void AfterElementFlushing(T element)
		{
			var index = _queue?.FindIndex(pair => EqualityComparer<T>.Default.Equals(pair.Value, element));
			if (!index.HasValue || index < 0)
			{
				GetOrCreateFlushedElements().Add(element);
			}
			else
			{
				_queue.RemoveAt(index.Value);
				_queueSize--;
			}
		}

		/// <inheritdoc />
		public override int CalculateDatabaseElementIndex(int index)
		{
			var dbIndex = index;

			if (_queue != null)
			{
				foreach (var pair in _queue)
				{
					if (pair.Key == index)
					{
						return -1; // The given index represents a queued value
					}

					if (pair.Key > index)
					{
						break;
					}

					dbIndex--;
				}
			}

			if (_removedDbIndexes != null)
			{
				var i = GetQueueIndex(ref _removedDbIndexes, new KeyValuePair<int, T>(dbIndex, default(T)), true);
				if (i < 0)
				{
					dbIndex += (Math.Abs(i) - 1);
				}
				else
				{
					i++;
					dbIndex += i;

					// Increase until we find a non removed db index
					for (; i < _removedDbIndexes.Count; i++)
					{
						if (_removedDbIndexes[i].Key != dbIndex)
						{
							break;
						}

						dbIndex++;
					}
				}
			}

			return dbIndex >= DatabaseCollectionSize
				? -1
				: dbIndex;
		}

		/// <inheritdoc />
		public override bool TryGetElementAtIndex(int index, out T element)
		{
			var pair = new KeyValuePair<int, T>(index, default(T));
			var keyIndex = GetQueueIndex(ref _queue, pair, true);
			if (keyIndex < 0)
			{
				element = default(T);
				return false;
			}

			element = _queue[keyIndex].Value;
			return true;
		}

		/// <inheritdoc />
		public override void RemoveElementAtIndex(int index, T element)
		{
			if (index >= GetCollectionSize())
			{
				// Mimic list behavior
				throw new ArgumentOutOfRangeException(
					nameof(index),
					"Index was out of range. Must be non-negative and less than the size of the collection.");
			}

			var pair = new KeyValuePair<int, T>(index, default(T));
			var i = GetQueueIndex(ref _queue, pair, true);
			if (i < 0)
			{
				i = Math.Abs(i) - 1;
				var dbIndex = CalculateDatabaseElementIndex(index);
				var removedPair = new KeyValuePair<int, T>(dbIndex, element);
				var j = GetQueueIndex(ref _removedDbIndexes, removedPair, true);
				if (j < 0)
				{
					_removedDbIndexes.Insert(Math.Abs(j) - 1, removedPair);
				}
			}
			else
			{
				_queue.RemoveAt(i);
			}

			_queueSize--;
			GetOrCreateRemovalQueue().Add(element);

			// We have to decrement all higher indexes by 1
			for (; i < _queue.Count; i++)
			{
				var currentPair = _queue[i];
				_queue[i] = new KeyValuePair<int, T>(currentPair.Key - 1, currentPair.Value);
			}
		}

		/// <inheritdoc />
		public override void AddElementAtIndex(int index, T element)
		{
			if (index > GetCollectionSize())
			{
				// Mimic list behavior
				throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the List.");
			}

			if (_flushedElements?.Remove(element) == true)
			{
				return;
			}

			var pair = new KeyValuePair<int, T>(index, element);
			var i = GetQueueIndex(ref _queue, pair, false);
			_queue.Insert(i, pair);
			i++;
			_queueSize++;
			_removalQueue?.Remove(element);

			// We have to increment all higher indexes by 1
			for (; i < _queue.Count; i++)
			{
				var currentPair = _queue[i];
				_queue[i] = new KeyValuePair<int, T>(currentPair.Key + 1, currentPair.Value);
			}
		}

		/// <inheritdoc />
		public override void SetElementAtIndex(int index, T element, T oldElement)
		{
			if (index >= GetCollectionSize())
			{
				// Mimic list behavior
				throw new ArgumentOutOfRangeException(
					nameof(index),
					"Index was out of range. Must be non-negative and less than the size of the collection.");
			}

			var pair = new KeyValuePair<int, T>(index, element);

			// NOTE: If we would need to have only items that are removed in the removal queue we would need
			// to check the _queue if the item already exists as the item can be readded to a different index.
			// As this scenario should rarely happen and we also know that ContainsElement will be called before
			// IsElementQueuedForDelete, we skip the check here to have a better performance.
			GetOrCreateRemovalQueue().Add(oldElement);
			_removalQueue.Remove(element);

			var i = GetQueueIndex(ref _queue, pair, true);
			if (i < 0)
			{
				var dbIndex = CalculateDatabaseElementIndex(index);
				var removedPair = new KeyValuePair<int, T>(dbIndex, oldElement);
				var j = GetQueueIndex(ref _removedDbIndexes, removedPair, true);
				if (j < 0)
				{
					_removedDbIndexes.Insert(Math.Abs(j) - 1, removedPair);
				}

				_queue.Insert(Math.Abs(i) - 1, pair);
			}
			else
			{
				_queue[i] = pair;
			}
		}

		private static int GetQueueIndex(ref List<KeyValuePair<int, T>> queue, KeyValuePair<int, T> pair, bool rawResult)
		{
			return GetQueueIndex(ref queue, pair, rawResult, Comparer);
		}

		private static int GetQueueIndex<TType>(ref List<TType> queue, TType item, bool rawResult, IComparer<TType> comparer)
		{
			if (queue == null)
			{
				queue = new List<TType>();
			}

			var i = queue.BinarySearch(item, comparer);
			if (i < 0)
			{
				return rawResult ? i : Math.Abs(i) - 1;
			}

			return i;
		}

		private ISet<T> GetOrCreateRemovalQueue()
		{
			return _removalQueue ?? (_removalQueue = new HashSet<T>());
		}

		private ISet<T> GetOrCreateFlushedElements()
		{
			return _flushedElements ?? (_flushedElements = new HashSet<T>());
		}
	}
}
