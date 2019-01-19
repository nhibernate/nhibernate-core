using System;
using System.Collections.Generic;

namespace NHibernate.Type
{
	[Serializable]
	public class GenericSortedListType<TKey, TValue> : GenericMapType<TKey, TValue>
	{
		private readonly IComparer<TKey> comparer;

		public GenericSortedListType(string role, string propertyRef, IComparer<TKey> comparer)
			: this(role, propertyRef, comparer, null, null, false)
		{
		}
		
		public GenericSortedListType(string role, string propertyRef, IComparer<TKey> comparer, string entityName, string propertyName, bool isNullable)
			: base(role, propertyRef, entityName, propertyName, isNullable)
		{
			this.comparer = comparer;
		}

		public IComparer<TKey> Comparer
		{
			get { return comparer; }
		}

		public override object Instantiate(int anticipatedSize)
		{
			return new SortedList<TKey, TValue>(comparer);
		}
	}
}
