using System;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A bag permits duplicates, so it has no primary key
	/// </summary>
	[Serializable]
	public class Bag : Collection
	{
		/// <summary>
		/// A bag permits duplicates, so it has no primary key.
		/// </summary>
		/// <param name="owner">The <see cref="PersistentClass"/> that contains this bag mapping.</param>
		public Bag(PersistentClass owner) : base(owner)
		{
		}

		/// <summary>
		/// Gets the appropriate <see cref="CollectionType"/> that is 
		/// specialized for this bag mapping.
		/// </summary>
		public override CollectionType DefaultCollectionType
		{
			get
			{
				if (IsGeneric)
				{
					CheckGenericArgumentsLength(1);
					return TypeFactory.GenericBag(Role, ReferencedPropertyName, GenericArguments[0]);
				}
				else
				{
					return TypeFactory.Bag(Role, ReferencedPropertyName, Embedded);
				}
			}
		}

		public override void CreatePrimaryKey()
		{
			//create an index on the key columns??
		}
	}
}
