using System;
using System.Collections.Generic;

namespace NHibernate.Type
{
	[Serializable]
	public class GenericSortedDictionaryType<TKey, TValue> : GenericMapType<TKey, TValue>
	{
		private readonly IComparer<TKey> comparer;

		public GenericSortedDictionaryType(string role, string propertyRef, IComparer<TKey> comparer)
			: base(role, propertyRef)
		{
			this.comparer = comparer;
		}

		public IComparer<TKey> Comparer
		{
			get { return comparer; }
		}

		public override object Instantiate(int anticipatedSize)
		{
			return new SortedDictionary<TKey, TValue>(comparer);
		}
	}
}
