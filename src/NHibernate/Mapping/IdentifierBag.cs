using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An <c>IdentifierBag</c> has a primary key consistenting of just
	/// the identifier column.
	/// </summary>
	public class IdentifierBag : IdentifierCollection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		public IdentifierBag( PersistentClass owner ) : base( owner )
		{
		}

		/// <summary></summary>
		public override PersistentCollectionType Type
		{
			get { return TypeFactory.IdBag( Role ); }
		}

		/// <summary></summary>
		public override System.Type WrapperClass
		{
			get { return typeof( NHibernate.Collection.IdentifierBag ); }
		}

	}
}