using System;
using System.Collections;
using Iesi.Collections;
using NHibernate.Engine;

namespace NHibernate.Collection
{
	/// <summary>
	/// A Persistent wrapper for a <c>Iesi.Collections.ISet</c> that has
	/// Set logic to prevent duplicate elements.
	/// </summary>
	/// <remarks>
	/// This class uses the Iesi.Collections.SortedSet for the SortedSet.  
	/// </remarks>
	[Serializable]
	public class SortedSet : Set, ISet
	{
		private IComparer comparer;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		protected override object Snapshot( CollectionPersister persister )
		{
			SortedList clonedSet = new SortedList( comparer, internalSet.Count );
			foreach( object obj in internalSet )
			{
				object copy = persister.ElementType.DeepCopy( obj );
				clonedSet.Add( copy, copy );
			}

			return clonedSet;
		}

		/// <summary></summary>
		public IComparer Comparer
		{
			get { return comparer; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		public override void BeforeInitialize( CollectionPersister persister )
		{
			internalSet = new Iesi.Collections.SortedSet( Comparer );
			// an ArrayList of the identifiers is what Set uses because there is not
			// both a Key & Value to worry about - just the Key.
			this.tempIdentifierList = new ArrayList();
		}

		/// <summary>
		/// Constuct a new empty SortedSet that uses a IComparer to perform the sorting.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="comparer">The IComparer to user for Sorting.</param>
		public SortedSet( ISessionImplementor session, IComparer comparer ) : base( session )
		{
			this.comparer = comparer;
		}

		/// <summary>
		/// Construct a new SortedSet initialized with the map values.
		/// </summary>
		/// <param name="session">The Session to be bound to.</param>
		/// <param name="map">The initial values.</param>
		/// <param name="comparer">The IComparer to use for Sorting.</param>
		public SortedSet( ISessionImplementor session, ISet map, IComparer comparer )
			: base( session, new Iesi.Collections.SortedSet( map, comparer ) )
		{
			this.comparer = comparer;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="persister"></param>
		/// <param name="comparer"></param>
		/// <param name="disassembled"></param>
		/// <param name="owner"></param>
		public SortedSet( ISessionImplementor session, CollectionPersister persister, IComparer comparer, object disassembled, object owner ) : this( session, comparer )
		{
			BeforeInitialize( persister );
			object[ ] array = ( object[ ] ) disassembled;
			for( int i = 0; i < array.Length; i++ )
			{
				object newObject = persister.ElementType.Assemble( array[ i ], session, owner );
				internalSet.Add( newObject );
			}

			initialized = true;
		}
	}
}