using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Iesi.Collections.Generic;
using NHibernate.Persister.Collection;
using NHibernate.Engine;

namespace NHibernate.Collection.Generic
{
	public class PersistentGenericSortedSet<T> : PersistentGenericSet<T>
	{
		private IComparer<T> comparer;

		public IComparer<T> Comparer
		{
			get { return comparer; }
		}

		public override void BeforeInitialize( ICollectionPersister persister )
		{
			internalSet = new SortedSet<T>( comparer );
		}

		protected override ICollection Snapshot( ICollectionPersister persister )
		{
			// NH: I think the snapshot does not necessarily have to be sorted, but
			// Hibernate uses a sorted collection, so we do too.
			SortedList<T, T> clonedSet = new SortedList<T, T>( internalSet.Count, comparer );
			foreach( object obj in internalSet )
			{
				T copy = ( T ) persister.ElementType.DeepCopy( obj );
				clonedSet.Add( copy, copy );
			}

			return clonedSet;
		}

		public PersistentGenericSortedSet( ISessionImplementor session, IComparer<T> comparer )
			: base( session )
		{
			this.comparer = comparer;
		}

		public PersistentGenericSortedSet( ISessionImplementor session, ISet<T> set, IComparer<T> comparer )
			: base( session, set )
		{
			this.comparer = comparer;
		}
	}
}
