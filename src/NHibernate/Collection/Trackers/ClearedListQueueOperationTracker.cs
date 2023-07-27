using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Persister.Collection;

namespace NHibernate.Collection.Trackers
{
	/// <inheritdoc />
	internal class ClearedListQueueOperationTracker<T> : AbstractCollectionQueueOperationTracker<T, IList<T>>
	{
		protected IList<T> Collection;
		private readonly ICollectionPersister _collectionPersister;

		public ClearedListQueueOperationTracker(ICollectionPersister collectionPersister)
		{
			_collectionPersister = collectionPersister;
		}

		/// <inheritdoc />
		public override bool AddElement(T element)
		{
			GetOrCreateQueue().Add(element);
			return true;
		}

		/// <inheritdoc />
		public override void RemoveExistingElement(T element, bool? existsInDb)
		{
			Collection?.Remove(element);
		}

		/// <inheritdoc />
		public override bool Cleared
		{
			get => true;
			protected set => throw new NotSupportedException();
		}

		public override void AfterFlushing()
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc />
		public override void ClearCollection()
		{
			Collection?.Clear();
		}

		/// <inheritdoc />
		public override bool ContainsElement(T element)
		{
			return Collection?.Contains(element) ?? false;
		}

		/// <inheritdoc />
		public override int GetQueueSize()
		{
			return Collection?.Count ?? 0;
		}

		/// <inheritdoc />
		public override bool IsElementQueuedForDelete(T element)
		{
			return false;
		}

		/// <inheritdoc />
		public override bool HasChanges()
		{
			return true;
		}

		/// <inheritdoc />
		public override void ApplyChanges(IList<T> loadedCollection)
		{
			loadedCollection.Clear();
			if (Collection != null)
			{
				foreach (var toAdd in Collection)
				{
					loadedCollection.Add(toAdd);
				}
			}
		}

		/// <inheritdoc />
		public override int? GetDatabaseElementIndex(int index)
		{
			return null;
		}

		/// <inheritdoc />
		public override bool TryGetElementAtIndex(int index, out T element)
		{
			if (Collection == null || index < 0 || index >= Collection.Count)
			{
				element = default(T);
				return false;
			}

			element = Collection[index];
			return true;
		}

		/// <inheritdoc />
		public override void RemoveElementAtIndex(int index, T element)
		{
			Collection?.RemoveAt(index);
		}

		/// <inheritdoc />
		public override void AddElementAtIndex(int index, T element)
		{
			GetOrCreateQueue().Insert(index, element);
		}

		/// <inheritdoc />
		public override void SetElementAtIndex(int index, T element, T oldElement)
		{
			GetOrCreateQueue()[index] = element;
		}

		/// <inheritdoc />
		public override IEnumerable GetAddedElements()
		{
			return Collection ?? (IEnumerable) Enumerable.Empty<object>();
		}

		/// <inheritdoc />
		public override IEnumerable GetOrphans()
		{
			return Enumerable.Empty<object>();
		}

		private IList<T> GetOrCreateQueue()
		{
			return Collection ?? (Collection = (IList<T>) _collectionPersister.CollectionType.Instantiate(-1));
		}
	}
}
