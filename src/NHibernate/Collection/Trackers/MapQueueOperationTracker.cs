using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Persister.Collection;

namespace NHibernate.Collection.Trackers
{
	/// <inheritdoc />
	internal class MapQueueOperationTracker<TKey, TValue> : AbstractMapQueueOperationTracker<TKey, TValue>
	{
		private readonly ICollectionPersister _collectionPersister;
		private IDictionary<TKey, TValue> _queue;
		private IDictionary<TKey, TValue> _orphanMap;
		private ISet<TKey> _removalQueue;
		private int _queueSize;
		private ISet<TValue> _flushedElements;

		public MapQueueOperationTracker(ICollectionPersister collectionPersister)
		{
			_collectionPersister = collectionPersister;
		}

		public override void AfterFlushing()
		{
			_queue?.Clear();
			_removalQueue?.Clear();
			_queueSize = 0;
			_orphanMap = null;
		}

		/// <inheritdoc />
		public override void ClearCollection()
		{
			AfterFlushing();
			Cleared = true;
		}

		/// <inheritdoc />
		public override void AfterElementFlushing(object element)
		{
			var value = (TValue) element;
			if (!TryGetKeyForValue(value, out var key))
			{
				GetOrCreateFlushedElements().Add(value);
			}
			else
			{
				_queue.Remove(key);
				_queueSize--;
			}
		}

		/// <inheritdoc />
		public override IEnumerable GetAddedElements()
		{
			return _queue?.Values ?? (IEnumerable) Enumerable.Empty<object>();
		}

		/// <inheritdoc />
		public override IEnumerable GetOrphans()
		{
			return _orphanMap?.Values ?? (IEnumerable) Enumerable.Empty<object>();
		}

		/// <inheritdoc />
		public override void AddElementByKey(TKey elementKey, TValue element, bool exists)
		{
			if (_flushedElements?.Remove(element) == true)
			{
				return;
			}

			if (exists)
			{
				throw new ArgumentException("An item with the same key has already been added."); // Mimic dictionary behavior
			}

			_removalQueue?.Remove(elementKey); // We have to remove the key from the removal list when the element is readded
			GetOrCreateQueue().Add(elementKey, element);
			if (!Cleared)
			{
				_queueSize++;
			}
		}

		/// <inheritdoc />
		public override bool ContainsKey(TKey key)
		{
			return _queue?.ContainsKey(key) == true;
		}

		/// <inheritdoc />
		public override int GetQueueSize()
		{
			return Cleared ? (_queue?.Count ?? 0) : _queueSize;
		}

		/// <inheritdoc />
		public override bool IsElementKeyQueuedForDelete(TKey elementKey)
		{
			return _removalQueue?.Contains(elementKey) ?? false;
		}

		/// <inheritdoc />
		public override bool RemoveElementByKey(TKey elementKey, TValue oldElement, bool? existsInDb)
		{
			if (Cleared)
			{
				return _queue?.Remove(elementKey) ?? false;
			}

			// We can have the following scenarios:
			// 1. remove a key that exists in db and it is not in the queue and removal queue (decrease queue size)
			// 2. remove a key that exists in db and it is in the queue (decrease queue size)
			// 3. remove a key that does not exist in db and it is not in the queue (don't decrease queue size)
			// 4. remove a key that does not exist in db and it is in the queue (decrease queue size)
			// 5. remove a key that exists in db and it is in the removal queue (don't decrease queue size)

			// If the key is not present in the database and in the queue, do nothing
			if (existsInDb == false && _queue?.ContainsKey(elementKey) != true)
			{
				return false;
			}

			if (existsInDb == true)
			{
				GetOrCreateOrphanMap()[elementKey] = oldElement;
			}

			// We don't want to have non database keys in the removal queue
			if (_queue?.Remove(elementKey) == true |
				(_orphanMap?.ContainsKey(elementKey) == true && GetOrCreateRemovalQueue().Add(elementKey)))
			{
				_queueSize--;
				return true;
			}

			return false;
		}

		/// <inheritdoc />
		public override void SetElementByKey(TKey elementKey, TValue element, TValue oldElement, bool? existsInDb)
		{
			if (Cleared)
			{
				GetOrCreateQueue()[elementKey] = element;
				return;
			}

			// We can have the following scenarios:
			// 1. set a key that exists in db and it is not in the queue (don't increase queue size)
			// 2. set a key that exists in db and it is in the queue (don't increase queue size)
			// 3. set a key that does not exist in db and it is not in the queue (increase queue size)
			// 4. set a key that does not exist in db and it is in the queue (don't increase queue size)
			// 5. set a key that exists in db and it is in the removal queue (increase queue size)
			if ((existsInDb == false && _queue?.ContainsKey(elementKey) != true) || _removalQueue?.Remove(elementKey) == true)
			{
				_queueSize++;
			}

			if (existsInDb == true)
			{
				GetOrCreateOrphanMap()[elementKey] = oldElement;
			}

			GetOrCreateQueue()[elementKey] = element;
		}

		/// <inheritdoc />
		public override bool TryGetElementByKey(TKey elementKey, out TValue element)
		{
			if (_queue == null)
			{
				element = default(TValue);
				return false;
			}

			return _queue.TryGetValue(elementKey, out element);
		}

		/// <inheritdoc />
		public override bool TryGetDatabaseElementByKey(TKey elementKey, out TValue element)
		{
			if (_orphanMap == null)
			{
				element = default(TValue);
				return false;
			}

			return _orphanMap.TryGetValue(elementKey, out element);
		}

		/// <inheritdoc />
		public override bool HasChanges()
		{
			return Cleared || _removalQueue?.Count > 0 || _queue?.Count > 0;
		}

		/// <inheritdoc />
		public override void ApplyChanges(IDictionary<TKey, TValue> loadedMap)
		{
			if (Cleared)
			{
				loadedMap.Clear();
			}
			else if (_removalQueue != null)
			{
				foreach (var toRemove in _removalQueue)
				{
					loadedMap.Remove(toRemove);
				}
			}

			if (_queue != null)
			{
				foreach (var toAdd in _queue)
				{
					loadedMap[toAdd.Key] = toAdd.Value;
				}
			}
		}

		private bool TryGetKeyForValue(TValue value, out TKey key)
		{
			if (_queue == null)
			{
				key = default(TKey);
				return false;
			}

			foreach (var pair in _queue)
			{
				if (EqualityComparer<TValue>.Default.Equals(pair.Value, value))
				{
					key = pair.Key;
					return true;
				}
			}

			key = default(TKey);
			return false;
		}

		private IDictionary<TKey, TValue> GetOrCreateQueue()
		{
			return _queue ?? (_queue = (IDictionary<TKey, TValue>) _collectionPersister.CollectionType.Instantiate(-1));
		}

		private IDictionary<TKey, TValue> GetOrCreateOrphanMap()
		{
			return _orphanMap ?? (_orphanMap = (IDictionary<TKey, TValue>) _collectionPersister.CollectionType.Instantiate(-1));
		}

		private ISet<TKey> GetOrCreateRemovalQueue()
		{
			return _removalQueue ?? (_removalQueue = new HashSet<TKey>());
		}

		private ISet<TValue> GetOrCreateFlushedElements()
		{
			return _flushedElements ?? (_flushedElements = new HashSet<TValue>());
		}
	}
}
