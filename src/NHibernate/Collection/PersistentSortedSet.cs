using System;
using System.Collections;
using Iesi.Collections;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Collection
{
	/// <summary>
	/// A Persistent wrapper for a <see cref="Iesi.Collections.ISet" /> that has
	/// Set logic to prevent duplicate elements.
	/// </summary>
	/// <remarks>
	/// This class uses the <see cref="Iesi.Collections.SortedSet" /> for the
	/// underlying representation.
	/// </remarks>
	[Serializable]
	public class PersistentSortedSet : PersistentSet, ISet
	{
		private IComparer comparer;

		protected override ICollection Snapshot( ICollectionPersister persister )
		{
			SortedList clonedSet = new SortedList( comparer, internalSet.Count );
			foreach( object obj in internalSet )
			{
				object copy = persister.ElementType.DeepCopy( obj );
				clonedSet.Add( copy, copy );
			}

			return clonedSet;
		}

		public IComparer Comparer
		{
			get { return comparer; }
		}

		public override void BeforeInitialize( ICollectionPersister persister )
		{
			internalSet = new Iesi.Collections.SortedSet( Comparer );
		}

		/// <summary>
		/// Constuct a new empty PersistentSortedSet that uses a IComparer to perform the sorting.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="comparer">The IComparer to user for Sorting.</param>
		internal PersistentSortedSet( ISessionImplementor session, IComparer comparer ) : base( session )
		{
			this.comparer = comparer;
		}

		/// <summary>
		/// Construct a new PersistentSortedSet initialized with the map values.
		/// </summary>
		/// <param name="session">The Session to be bound to.</param>
		/// <param name="map">The initial values.</param>
		/// <param name="comparer">The IComparer to use for Sorting.</param>
		internal PersistentSortedSet( ISessionImplementor session, ISet map, IComparer comparer )
			: base( session, new Iesi.Collections.SortedSet( map, comparer ) )
		{
			this.comparer = comparer;
		}
	}
}