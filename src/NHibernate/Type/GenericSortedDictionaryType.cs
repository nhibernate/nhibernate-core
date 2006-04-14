using System;
using System.Collections.Generic;
using System.Text;

using NHibernate.Collection;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	public class GenericSortedDictionaryType<TKey, TValue> : GenericMapType<TKey, TValue>
	{
		private IComparer<TKey> comparer;

		public GenericSortedDictionaryType( string role, string propertyRef, IComparer<TKey> comparer )
			: base( role, propertyRef )
		{
			this.comparer = comparer;
		}

		public IComparer<TKey> Comparer
		{
			get { return comparer; }
		}

		public override IPersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
		{
			return new PersistentGenericSortedDictionary<TKey, TValue>( session, comparer );
		}

		public override IPersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new PersistentGenericSortedDictionary<TKey, TValue>( session, ( IDictionary<TKey, TValue> ) collection, comparer );
		}
	}
}
