using NHibernate.Type;
using NHibernateBag=NHibernate.Collection.Bag;

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
		public override PersistentCollectionType Type
		{
			get { return TypeFactory.Bag( Role ); }
		}

		/// <summary></summary>
		public override System.Type WrapperClass
		{
			get { return typeof( NHibernateBag ); }
		}

	}
}