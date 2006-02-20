using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Collection
{
	/// <summary>
	/// A Persistent wrapper for a <c>System.Collections.IDictionary</c> that has
	/// sorting.
	/// </summary>
	/// <remarks>
	/// This class uses the SortedList as the underlying map for the SortedMap.  The SortedList
	/// is not really an IList at all.  It actually is a Hashtable that provides methods to get
	/// to a Key by its index.  Since it is sorted the indexes can change based on what is added
	/// to the Dictionary.  In my opinion, the index is not useful except to get the first or last
	/// element.
	/// </remarks>
	[Serializable]
	public class SortedMap : Map, IDictionary
	{
		private IComparer comparer;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		protected override ICollection Snapshot( ICollectionPersister persister )
		{
			SortedList clonedMap = new SortedList( comparer );
			foreach( DictionaryEntry de in map )
			{
				object copy = persister.ElementType.DeepCopy( de.Value );
				clonedMap.Add( de.Key, copy );
			}

			return clonedMap;
		}

		/// <summary></summary>
		public IComparer Comparer
		{
			get { return comparer; }
		}

		/// <summary>
		/// Create an Uninitialized SortedMap.
		/// </summary>
		/// <param name="session">The ISession the Map should be a part of.</param>
		internal SortedMap( ISessionImplementor session )
			: base( session )
		{
		}

		/// <summary>
		/// Constuct an uninitialized SortedMap that uses an IComparer to perform the sorting.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="comparer">The IComparer to user for Sorting.</param>
		internal SortedMap( ISessionImplementor session, IComparer comparer )
			: base( session )
		{
			this.comparer = comparer;
		}

		/// <summary>
		/// Construct an initialized SortedMap based off the values from the existing IDictionary.
		/// </summary>
		/// <param name="session">The ISession the SortedMap should be a part of.</param>
		/// <param name="map">The IDictionary that contains the initial values.</param>
		/// <param name="comparer">The IComparer to use for Sorting.</param>
		internal SortedMap( ISessionImplementor session, IDictionary map, IComparer comparer )
			: base( session, new SortedList( map, comparer ) )
		{
			this.comparer = comparer;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		public override void BeforeInitialize( ICollectionPersister persister )
		{
			this.map = new SortedList( comparer );
		}
	}
}