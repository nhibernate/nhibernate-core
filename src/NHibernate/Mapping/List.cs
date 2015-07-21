using System;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A list has a primary key consisting of the key columns + index column
	/// </summary>
	[Serializable]
	public class List : IndexedCollection
	{
		private int baseIndex;

		/// <summary>
		/// Initializes a new instance of the <see cref="List"/> class.
		/// </summary>
		/// <param name="owner">The <see cref="PersistentClass"/> that contains this list mapping.</param>
		public List(PersistentClass owner) : base(owner)
		{
		}

		/// <summary>
		/// Gets the appropriate <see cref="CollectionType"/> that is 
		/// specialized for this list mapping.
		/// </summary>
		public override CollectionType DefaultCollectionType
		{
			get
			{
				if (IsGeneric)
				{
					CheckGenericArgumentsLength(1);
					return TypeFactory.GenericList(Role, ReferencedPropertyName, GenericArguments[0]);
				}

				throw new MappingException("Non-generic persistent lists are not supported (role " + Role +").");
			}
		}
		
		public int BaseIndex
		{
			get { return baseIndex; }
			set { baseIndex = value; }
		}

		public override bool IsList
		{
			get { return true; }
		}
	}
}
