using System;
using System.Collections.Generic;

using NHibernate.Persister.Collection;
using NHibernate.Engine;

namespace NHibernate.Collection.Generic
{
	public class PersistentGenericSortedList<TKey, TValue> : PersistentGenericMap<TKey, TValue>
	{
		private IComparer<TKey> comparer;

		public PersistentGenericSortedList( ISessionImplementor session, IComparer<TKey> comparer )
			: base( session )
		{
			this.comparer = comparer;
		}

		public PersistentGenericSortedList( ISessionImplementor session, IDictionary<TKey, TValue> map, IComparer<TKey> comparer )
			: base( session, map )
		{
			this.comparer = comparer;
		}

		public override void BeforeInitialize( ICollectionPersister persister )
		{
			this.map = new SortedList<TKey, TValue>( comparer );
		}
	}
}
