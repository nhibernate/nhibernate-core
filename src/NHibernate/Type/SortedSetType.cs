using System;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type{
	/// <summary>
	/// Extends the SetType to provide Sorting.
	/// </summary>
	public class SortedSetType : SetType {
		
		private IComparer comparer;

		public SortedSetType(string role, IComparer comparer) : base(role){
			this.comparer = comparer;
		}

		public override PersistentCollection Instantiate(ISessionImplementor session, CollectionPersister persister) {
			SortedSet sortedSet = new SortedSet(session, comparer);
			//sortedSet.Comparer = comparer;
			return sortedSet;
		}

		//public System.Type ReturnedClass {get;} -> was overridden in H2.0.3
		// because they have different Interfaces for Sorted/UnSorted - since .NET
		// doesn't have that I don't need to override it.

		public override PersistentCollection Wrap(ISessionImplementor session, object collection) {
			return new SortedSet(session, (IDictionary)collection, comparer);
			
		}

		public override PersistentCollection AssembleCachedCollection(ISessionImplementor session, CollectionPersister persister, object disassembled, object owner) {
			return new SortedSet(session, persister, comparer, disassembled, owner);
		}


			
	}
}
