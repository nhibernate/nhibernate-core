using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An <c>PersistentIdentifierBag</c> has a primary key consistenting of just
	/// the identifier column.
	/// </summary>
	public class IdentifierBag : IdentifierCollection
	{
		public IdentifierBag(PersistentClass owner) : base(owner)
		{
		}

		public override CollectionType DefaultCollectionType
		{
			get
			{
				if (this.IsGeneric)
				{
					CheckGenericArgumentsLength(1);
					return TypeFactory.GenericIdBag(Role, ReferencedPropertyName, GenericArguments[0]);
				}
				return TypeFactory.IdBag(Role, ReferencedPropertyName);
			}
		}
	}
}
