using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An <c>PersistentIdentifierBag</c> has a primary key consistenting of just
	/// the identifier column.
	/// </summary>
	public class IdentifierBag : IdentifierCollection
	{
		public IdentifierBag( PersistentClass owner ) : base( owner )
		{
		}

		public override CollectionType CollectionType
		{
			get
			{
#if NET_2_0
				if( this.IsGeneric )
				{
					return TypeFactory.GenericIdBag( Role, ReferencedPropertyName, GenericArguments[ 0 ] );
				}
#endif
				return TypeFactory.IdBag( Role, ReferencedPropertyName );
			}
		}
	}
}