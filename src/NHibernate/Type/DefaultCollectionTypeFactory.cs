using System.Collections;
using System.Collections.Generic;
using NHibernate.Bytecode;

namespace NHibernate.Type
{
	public class DefaultCollectionTypeFactory : ICollectionTypeFactory
	{
		public virtual CollectionType Array(string role, string propertyRef, bool embedded, System.Type elementClass)
		{
			return new ArrayType(role, propertyRef, elementClass, embedded);
		}

		public virtual CollectionType Bag<T>(string role, string propertyRef, bool embedded)
		{
			return new GenericBagType<T>(role, propertyRef);
		}

		public virtual CollectionType List<T>(string role, string propertyRef, bool embedded)
		{
			return new GenericListType<T>(role, propertyRef);
		}

		public virtual CollectionType IdBag<T>(string role, string propertyRef, bool embedded)
		{
			return new GenericIdentifierBagType<T>(role, propertyRef);
		}

		public virtual CollectionType Set<T>(string role, string propertyRef, bool embedded)
		{
			return new GenericSetType<T>(role, propertyRef);
		}

		public virtual CollectionType SortedSet<T>(string role, string propertyRef, bool embedded, IComparer<T> comparer)
		{
			return new GenericSortedSetType<T>(role, propertyRef, comparer);
		}

		public virtual CollectionType OrderedSet<T>(string role, string propertyRef, bool embedded)
		{
			return new GenericOrderedSetType<T>(role, propertyRef);
		}

		public virtual CollectionType Map<TKey, TValue>(string role, string propertyRef, bool embedded)
		{
			return new GenericMapType<TKey, TValue>(role, propertyRef);
		}

		public virtual CollectionType SortedDictionary<TKey, TValue>(string role, string propertyRef, bool embedded, IComparer<TKey> comparer)
		{
			return new GenericSortedDictionaryType<TKey, TValue>(role, propertyRef, comparer);
		}

		public virtual CollectionType SortedList<TKey, TValue>(string role, string propertyRef, bool embedded, IComparer<TKey> comparer)
		{
			return new GenericSortedListType<TKey, TValue>(role, propertyRef, comparer);
		}
	}
}