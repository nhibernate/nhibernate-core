using NHibernate.Collection;
using NHibernate.Type;
using Collection_Map = NHibernate.Collection.Map;
using NHCollection = NHibernate.Collection;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class Map : IndexedCollection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		public Map( PersistentClass owner ) : base( owner )
		{
		}

		/// <summary></summary>
		public override PersistentCollectionType Type
		{
			get
			{
				return IsSorted ?
					TypeFactory.SortedMap( Role, Comparer ) :
					TypeFactory.Map( Role );
			}
		}

		/// <summary></summary>
		public override System.Type WrapperClass
		{
			get
			{
				return IsSorted ?
					typeof( SortedMap ) :
					typeof( Collection_Map );
			}
		}
	}
}