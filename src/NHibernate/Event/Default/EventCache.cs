using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Event.Default
{
	public class EventCache : IDictionary
	{
		private IDictionary<object, object> entityToCopyMap = IdentityMapUtils.Instantiate<object, object>(10);
		// key is an entity involved with the operation performed by the listener;
		// value can be either a copy of the entity or the entity itself

		private IDictionary<object, object> entityToOperatedOnFlagMap = IdentityMapUtils.Instantiate<object, object>(10);
		// key is an entity involved with the operation performed by the listener;
		// value is a flag indicating if the listener explicitly operates on the entity

		#region ICollection Implementation

		/// <summary>
		/// Returns the number of entity-copy mappings in this EventCache
		/// </summary>
		public int Count
		{
			get { return entityToCopyMap.Count; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (index < 0)
				throw new ArgumentOutOfRangeException("arrayIndex is less than 0");
			if (entityToCopyMap.Count + index + 1 > array.Length)
				throw new ArgumentException("The number of elements in the source ICollection<T> is greater than the available space from arrayIndex to the end of the destination array.");

			var i = index;
			foreach (var entry in entityToCopyMap)
			{
				array.SetValue(new DictionaryEntry(entry.Key, entry.Value), i++);
			}
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion

		#region IDictionary implementation

		public object this[object key]
		{
			get => entityToCopyMap.TryGetValue(key, out var value) ? value : null;
			set => Add(key, value);
		}

		public bool IsReadOnly => false;

		public bool IsFixedSize => false;

		public ICollection Keys => new CollectionAdapter(entityToCopyMap.Keys);

		public ICollection Values => new CollectionAdapter(entityToCopyMap.Values);

		public void Add(object key, object value)
		{
			if (key == null)
				throw new ArgumentNullException("key");
			if (value == null)
				throw new ArgumentNullException("value");

			entityToCopyMap.Add(key, value);
			entityToOperatedOnFlagMap.Add(key, false);
		}

		public bool Contains(object key)
		{
			return entityToCopyMap.ContainsKey(key);
		}

		public void Remove(object key)
		{
			entityToCopyMap.Remove(key);
			entityToOperatedOnFlagMap.Remove(key);
		}

		public IDictionaryEnumerator GetEnumerator() => new DictionaryEnumeratorAdapter(entityToCopyMap.GetEnumerator());

		public void Clear()
		{
			entityToCopyMap.Clear();
			entityToOperatedOnFlagMap.Clear();
		}

		#endregion

		/// <summary>
		/// Associates the specified entity with the specified copy in this EventCache;
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="copy"></param>
		/// <param name="isOperatedOn">indicates if the operation is performed on the entity</param>
		public void Add(object entity, object copy, bool isOperatedOn)
		{
			if (entity == null)
				throw new ArgumentNullException("null entities are not supported", "entity");
			if (copy == null)
				throw new ArgumentNullException("null entity copies are not supported", "copy");

			entityToCopyMap.Add(entity, copy);
			entityToOperatedOnFlagMap.Add(entity, isOperatedOn);
		}

		/// <summary>
		/// Returns copy-entity mappings
		/// </summary>
		/// <returns></returns>
		public IDictionary InvertMap()
		{
			IDictionary result = IdentityMap.Instantiate(entityToCopyMap.Count);
			foreach (var entry in entityToCopyMap)
			{
				result[entry.Value] = entry.Key;
			}
			return result;
		}

		/// <summary>
		/// Returns true if the listener is performing the operation on the specified entity.
		/// </summary>
		/// <param name="entity">Must be non-null and this EventCache must contain a mapping for this entity</param>
		/// <returns></returns>
		public bool IsOperatedOn(object entity)
		{
			if (entity == null)
				throw new ArgumentNullException("null entities are not supported", "entity");

			return (bool) entityToOperatedOnFlagMap[entity];
		}

		/// <summary>
		/// Set flag to indicate if the listener is performing the operation on the specified entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="isOperatedOn"></param>
		public void SetOperatedOn(object entity, bool isOperatedOn)
		{
			if (entity == null)
				throw new ArgumentNullException("null entities are not supported", "entity");

			if (!entityToOperatedOnFlagMap.ContainsKey(entity) || !entityToCopyMap.ContainsKey(entity))
				throw new AssertionFailure("called EventCache.SetOperatedOn() for entity not found in EventCache");

			entityToOperatedOnFlagMap[entity] = isOperatedOn;
		}

		/// <summary>
		/// Adapts a generic <see cref="IEnumerator{T}"/> over <see cref="KeyValuePair{TKey,TValue}"/> to the
		/// classic <see cref="IDictionaryEnumerator"/> shape expected by consumers of <see cref="EventCache"/>'s
		/// public non-generic <see cref="IDictionary"/> implementation.
		/// </summary>
		private sealed class DictionaryEnumeratorAdapter : IDictionaryEnumerator
		{
			private readonly IEnumerator<KeyValuePair<object, object>> _wrapped;

			public DictionaryEnumeratorAdapter(IEnumerator<KeyValuePair<object, object>> wrapped) => _wrapped = wrapped;

			public bool MoveNext() => _wrapped.MoveNext();

			public void Reset() => _wrapped.Reset();

			public object Current => Entry;

			public DictionaryEntry Entry => new(_wrapped.Current.Key, _wrapped.Current.Value);

			public object Key => _wrapped.Current.Key;

			public object Value => _wrapped.Current.Value;
		}

		/// <summary>
		/// Adapts a generic <see cref="ICollection{T}"/> to the classic non-generic <see cref="ICollection"/>
		/// shape expected for <see cref="EventCache"/>'s public <see cref="IDictionary.Keys"/>/<see cref="IDictionary.Values"/>.
		/// </summary>
		private sealed class CollectionAdapter : ICollection
		{
			private readonly ICollection<object> _wrapped;

			public CollectionAdapter(ICollection<object> wrapped) => _wrapped = wrapped;

			public int Count => _wrapped.Count;

			public bool IsSynchronized => false;

			public object SyncRoot => this;

			public void CopyTo(Array array, int index)
			{
				foreach (var item in _wrapped)
				{
					array.SetValue(item, index++);
				}
			}

			public IEnumerator GetEnumerator() => _wrapped.GetEnumerator();
		}
	}
}
