#if NET_2_0
using System;
using System.Collections.Generic;

namespace NHibernate.Type
{
	[Serializable]
	public class GenericSortedListType<TKey, TValue> : GenericMapType<TKey, TValue>
	{
		private IComparer<TKey> comparer;

		public GenericSortedListType(string role, string propertyRef, IComparer<TKey> comparer)
			: base(role, propertyRef)
		{
			this.comparer = comparer;
		}

		public IComparer<TKey> Comparer
		{
			get { return comparer; }
		}

		public override object Instantiate()
		{
			return new SortedList<TKey, TValue>(comparer);
		}
	}
}

#endif