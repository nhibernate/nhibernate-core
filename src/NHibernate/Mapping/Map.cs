using System;

using NHibernate.Type;
using NHCollection = NHibernate.Collection;


namespace NHibernate.Mapping 
{	
	public class Map : IndexedCollection 
	{
		public Map(PersistentClass owner) : base(owner) { }

		public override PersistentCollectionType Type 
		{
			get 
			{ 
				return IsSorted ? 
					TypeFactory.SortedMap( Role, Comparer ) : 
					TypeFactory.Map( Role ); 
			}
		}

		public override System.Type WrapperClass 
		{
			get 
			{ 
				return IsSorted ? 
					typeof(NHCollection.SortedMap) : 
					typeof(NHCollection.Map); 
			}
		}
	}
}
