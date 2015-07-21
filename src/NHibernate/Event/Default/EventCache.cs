using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Event.Default
{
	public class EventCache : IDictionary
	{
		private IDictionary entityToCopyMap = IdentityMap.Instantiate(10);
		// key is an entity involved with the operation performed by the listener;
		// value can be either a copy of the entity or the entity itself
	
		private IDictionary entityToOperatedOnFlagMap = IdentityMap.Instantiate(10);
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
		
			entityToCopyMap.CopyTo(array, index);
		}
		
		#endregion
		
		#region IEnumerable implementation
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)entityToCopyMap).GetEnumerator();
		}

		#endregion
		
		#region IDictionary implementation
		
		public object this[object key]
		{
			get 
			{
				return entityToCopyMap[key];
			}
			set
			{
				this.Add(key, value);
			}
		}
		
		public bool IsReadOnly
		{
			get { return false; }
		}
		
		public bool IsFixedSize
		{
			get { return false; }
		}

		public ICollection Keys
		{
			get { return entityToCopyMap.Keys; }
		}
		
		public ICollection Values
		{
			get { return entityToCopyMap.Values; }
		}
		
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
			return entityToCopyMap.Contains(key);
		}
		
		public void Remove(object key)
		{
			entityToCopyMap.Remove(key);
			entityToOperatedOnFlagMap.Remove(key);
		}
		
		public IDictionaryEnumerator GetEnumerator()
		{
			return entityToCopyMap.GetEnumerator();
		}
		
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
			return IdentityMap.Invert(entityToCopyMap);
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

			return (bool)entityToOperatedOnFlagMap[entity];
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

			if (!entityToOperatedOnFlagMap.Contains(entity) || !entityToCopyMap.Contains(entity))
				throw new AssertionFailure("called EventCache.SetOperatedOn() for entity not found in EventCache");

			entityToOperatedOnFlagMap[entity] = isOperatedOn;
		}
	}
}