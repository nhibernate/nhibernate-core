using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A map has a primary key consisting of the key columns 
	/// + index columns.
	/// </summary>
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
		public override PersistentCollectionType CollectionType
		{
			get
			{
				return IsSorted ?
					TypeFactory.SortedMap( Role, Comparer ) :
					TypeFactory.Map( Role );
			}
		}

		/// <summary></summary>
		public override void CreateAllKeys( )
		{
			base.CreateAllKeys();
			if ( !IsInverse )
			{
				Index.CreateForeignKey();
			}
		}
	}
}