using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A bag permits duplicates, so it has no primary key
	/// </summary>
	public class Bag : Collection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		public Bag( PersistentClass owner ) : base( owner )
		{
		}

		/// <summary></summary>
		public override PersistentCollectionType CollectionType
		{
			get { return TypeFactory.Bag( Role ); }
		}

		/// <summary></summary>
		public override System.Type WrapperClass
		{
			get { return typeof( NHibernate.Collection.Bag ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>Should we create an index on the key columns?</remarks>
		public override void CreatePrimaryKey( )
		{
		}
	}
}