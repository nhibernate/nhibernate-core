using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A list has a primary key consisting of the key columns + index column
	/// </summary>
	public class List : IndexedCollection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		public List( PersistentClass owner ) : base( owner )
		{
		}

		/// <summary></summary>
		public override PersistentCollectionType CollectionType
		{
			get { return TypeFactory.List( Role ); }
		}

		/// <summary></summary>
		public override System.Type WrapperClass
		{
			get { return typeof( NHibernate.Collection.List ); }
		}

	}
}