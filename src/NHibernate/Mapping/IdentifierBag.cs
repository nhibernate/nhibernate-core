using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An <c>PersistentIdentifierBag</c> has a primary key consistenting of just
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
		public override PersistentCollectionType CollectionType
		{
			get { return TypeFactory.IdBag( Role, ReferencedPropertyName ); }
		}
	}
}