using System;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An <c>PersistentIdentifierBag</c> has a primary key consistenting of just
	/// the identifier column.
	/// </summary>
	[Serializable]
	public class IdentifierBag : IdentifierCollection
	{
		public IdentifierBag(PersistentClass owner)
			: base(owner)
		{
		}

		public override CollectionType DefaultCollectionType
		{
			get
			{
				System.Type elementType = typeof(object);
				if (IsGeneric)
				{
					CheckGenericArgumentsLength(1);
					elementType = GenericArguments[0];
				}

				return TypeFactory.GenericIdBag(Role, ReferencedPropertyName, elementType);
			}
		}
	}
}
