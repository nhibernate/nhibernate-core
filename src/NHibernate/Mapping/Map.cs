using System;

using NHibernate.Type;
using NHCollection = NHibernate.Collection;


namespace NHibernate.Mapping 
{	
	public class Map : IndexedCollection {
		
		public Map(PersistentClass owner) : base(owner) { }

		public override PersistentCollectionType Type 
		{
			//TODO: H2.0.3 - fix up when SortedMap is implemented.
			//get { return IsSorted ? TypeFactory.SortedMap( Role, Comparator ) : TypeFactory.Map( Role );
			get { return TypeFactory.Map( Role ); } //TODO: get sorted
		}

		public override System.Type WrapperClass 
		{
			//TODO: H2.0.3 - fix up when SortedMap is implemented.
			//get { return IsSorted ? typeof(NHCollection.SortedMap) : typeof(NHCollection.Map); }
			get { return typeof(NHCollection.Map); }
		}


	}
}
