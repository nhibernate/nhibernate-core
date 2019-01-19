using System;
using System.Collections.Generic;
using NHibernate.Bytecode;

namespace NHibernate.Type
{
	public class DefaultCollectionTypeFactory : ICollectionTypeFactory
	{
		[Obsolete("Use Array method with entityName, propertyName, isNullable")]
		public virtual CollectionType Array(string role, string propertyRef, System.Type elementClass)
		{
			return Array(role, propertyRef, elementClass, null, null, false);
		}
		public virtual CollectionType Array(string role, string propertyRef, System.Type elementClass, string entityName, string propertyName, bool isNullable)
		{
			return new ArrayType(role, propertyRef, elementClass, entityName, propertyName, isNullable);
		}

		[Obsolete("Use Bag method with entityName, propertyName, isNullable")]
		public virtual CollectionType Bag<T>(string role, string propertyRef)
		{
			return Bag<T>(role, propertyRef, null, null, false);
		}
		
		public virtual CollectionType Bag<T>(string role, string propertyRef, string entityName, string propertyName, bool isNullable)
		{
			return new GenericBagType<T>(role, propertyRef, entityName, propertyName, isNullable);
		}

		[Obsolete("Use List method with entityName, propertyName, isNullable")]
		public virtual CollectionType List<T>(string role, string propertyRef)
		{
			return List<T>(role, propertyRef, null, null, false);
		}
		
		public virtual CollectionType List<T>(string role, string propertyRef, string entityName, string propertyName, bool isNullable)
		{
			return new GenericListType<T>(role, propertyRef, entityName, propertyName, isNullable);
		}

		[Obsolete("Use IdBag method with entityName, propertyName, isNullable")]
		public virtual CollectionType IdBag<T>(string role, string propertyRef)
		{
			return IdBag<T>(role, propertyRef, null, null, false);
		}
		
		public virtual CollectionType IdBag<T>(string role, string propertyRef, string entityName, string propertyName, bool isNullable)
		{
			return new GenericIdentifierBagType<T>(role, propertyRef, entityName, propertyName, isNullable);
		}

		[Obsolete("Use Set method with entityName, propertyName, isNullable")]
		public virtual CollectionType Set<T>(string role, string propertyRef)
		{
			return Set<T>(role, propertyRef, null, null, false);
		}
		
		public virtual CollectionType Set<T>(string role, string propertyRef, string entityName, string propertyName, bool isNullable)
		{
			return new GenericSetType<T>(role, propertyRef, entityName, propertyName, isNullable);
		}

		[Obsolete("Use SortedSet method with entityName, propertyName, isNullable")]
		public virtual CollectionType SortedSet<T>(string role, string propertyRef, IComparer<T> comparer)
		{
			return SortedSet<T>(role, propertyRef, comparer, null, null, false);
		}
		
		public virtual CollectionType SortedSet<T>(string role, string propertyRef,IComparer<T> comparer,  string entityName, string propertyName, bool isNullable)
		{
			return new GenericSortedSetType<T>(role, propertyRef, comparer, entityName, propertyName, isNullable);
		}

		[Obsolete("Use OrderedSet method with entityName, propertyName, isNullable")]
		public virtual CollectionType OrderedSet<T>(string role, string propertyRef)
		{
			return OrderedSet<T>(role, propertyRef, null, null, false);
		}
		
		public virtual CollectionType OrderedSet<T>(string role, string propertyRef, string entityName, string propertyName, bool isNullable)
		{
			return new GenericOrderedSetType<T>(role, propertyRef, entityName, propertyName, isNullable);
		}

		[Obsolete("Use Map method with entityName, propertyName, isNullable")]
		public virtual CollectionType Map<TKey, TValue>(string role, string propertyRef)
		{
			return Map<TKey, TValue>(role, propertyRef, null, null, false);
		}
		
		public virtual CollectionType Map<TKey, TValue>(string role, string propertyRef, string entityName, string propertyName, bool isNullable)
		{
			return new GenericMapType<TKey, TValue>(role, propertyRef, entityName, propertyName, isNullable);
		}

		[Obsolete("Use SortedDictionary method with entityName, propertyName, isNullable")]
		public virtual CollectionType SortedDictionary<TKey, TValue>(string role, string propertyRef, IComparer<TKey> comparer)
		{
			return  SortedDictionary<TKey, TValue>(role, propertyRef, comparer, null, null, false);
		}
		
		public virtual CollectionType SortedDictionary<TKey, TValue>(string role, string propertyRef, IComparer<TKey> comparer, string entityName, string propertyName, bool isNullable)
		{
			return new GenericSortedDictionaryType<TKey, TValue>(role, propertyRef, comparer, entityName, propertyName, isNullable);
		}

		[Obsolete("Use SortedList method with entityName, propertyName, isNullable")]
		public virtual CollectionType SortedList<TKey, TValue>(string role, string propertyRef, IComparer<TKey> comparer)
		{
			return SortedList<TKey, TValue>(role, propertyRef, comparer, null, null, false);
		}
		
		public virtual CollectionType SortedList<TKey, TValue>(string role, string propertyRef, IComparer<TKey> comparer, string entityName, string propertyName, bool isNullable)
		{
			return new GenericSortedListType<TKey, TValue>(role, propertyRef, comparer, entityName, propertyName, isNullable);
		}
	}
}