using System;
using System.Collections;
using System.Data;

using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection 
{
	/// <summary>
	/// A Persistent wrapper for a <c>System.Collections.IDictionary</c> that has
	/// Set logic to prevent duplicate elements.
	/// </summary>
	/// <remarks>
	/// This class uses the SortedList as the underlying map for the SortedSet.  The SortedList
	/// is not really an IList at all.  It actually is a Hashtable that provides methods to get
	/// to a Key by its index.  Since it is sorted the indexes can change based on what is added
	/// to the Dictionary.  In my opinion, the index is not useful except to get the first or last
	/// element.
	/// </remarks>
	public class SortedSet : Set, IDictionary  
	{
		private IComparer comparer;

		protected override object Snapshot(CollectionPersister persister) 
		{
			SortedList clonedSet = new SortedList(comparer, map.Count);
			foreach(DictionaryEntry de in map) 
			{
				object copy = persister.ElementType.DeepCopy(de.Key);
				clonedSet.Add(copy, copy);
			}

			return clonedSet;
		}

		public IComparer Comparer 
		{
			get { return comparer;}
		}


		public override void BeforeInitialize(CollectionPersister persister) 
		{
			this.map = new SortedList(comparer); 
			// an ArrayList of the identifiers is what Set uses because there is not
			// both a Key & Value to worry about - just the Key.
			this.tempIdentifierList = new ArrayList();
		}

		/// <summary>
		/// Constuct a new empty SortedSet that uses a IComparer to perform the sorting.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="comparer">The IComparer to user for Sorting.</param>
		public SortedSet(ISessionImplementor session, IComparer comparer) : base(session)
		{
			this.comparer = comparer;
		}

		/// <summary>
		/// Construct a new SortedSet initialized with the map values.
		/// </summary>
		/// <param name="session">The Session to be bound to.</param>
		/// <param name="map">The initial values.</param>
		/// <param name="comparer">The IComparer to use for Sorting.</param>
		public SortedSet(ISessionImplementor session, IDictionary map, IComparer comparer) : base(session, new SortedList(map, comparer)) 
		{
			this.comparer = comparer;
		}

		public SortedSet(ISessionImplementor session, CollectionPersister persister, IComparer comparer, object disassembled, object owner) : this(session, comparer) 
		{
			BeforeInitialize(persister);
			object[] array = (object[])disassembled;
			for(int i = 0; i < array.Length; i++) 
			{
				object newObject = persister.ElementType.Assemble(array[i], session, owner);
				map.Add(newObject, newObject);
			}

			initialized = true;
		}


		//TODO: H2.0.3 has an internal class SubSetProxy that inherits from another
		// undefined class PersistentCollection.SetProxy

	}
}
