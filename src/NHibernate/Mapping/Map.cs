using System;
using NHibernate.Type;
using SortedMap = NHibernate.Collection.SortedMap;
using Map = NHibernate.Collection.Map;


namespace NHibernate.Mapping {
	
	public class Map : IndexedCollection {
		
		public Map(PersistentClass owner) : base(owner) { }

		public override PersistentCollectionType Type {
			//get { return IsSorted ? TypeFactory.SortedMap( Role, Comparator ) : TypeFactory.Map( Role );
			get { return TypeFactory.Map( Role ); } //TODO: get sorted
		}

		public override System.Type WrapperClass {
			get { return IsSorted ? typeof(SortedMap) : typeof(Map); }
		}


	}
}
