using System;
using System.Collections;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A map has a primary key consisting of the key columns 
	/// + index columns.
	/// </summary>
	[Serializable]
	public class Map : IndexedCollection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Map" /> class.
		/// </summary>
		/// <param name="owner">The <see cref="PersistentClass"/> that contains this map mapping.</param>
		public Map(PersistentClass owner) : base(owner)
		{
		}

		public override bool IsMap
		{
			get { return true; }
		}

		public override CollectionType CollectionType
		{
			get
			{
				if (IsGeneric && IsSorted)
				{
					CheckGenericArgumentsLength(2);
					if (TypeName.Contains("sorted-list"))
					{
						return
							TypeFactory.GenericSortedList(Role, ReferencedPropertyName, Comparer, GenericArguments[0], GenericArguments[1]);
					}
					else if (TypeName.Contains("sorted-dictionary"))
					{
						return
							TypeFactory.GenericSortedDictionary(Role, ReferencedPropertyName, Comparer, GenericArguments[0],
							                                    GenericArguments[1]);
					}
					else
					{
						throw new MappingException(
							"Use collection-type='sorted-list/sorted-dictionary' to choose implementation for generic map");
					}
				}
				return base.CollectionType;
			}
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
					if (HasOrder)
					{
						throw new MappingException(
							"Cannot use order-by with generic map, no appropriate collection implementation is available");
					}
					else if (IsSorted)
					{
						throw new AssertionFailure("Error in NH: should not get here (Mapping.Map.DefaultCollectionType)");
					}
					else
					{
						CheckGenericArgumentsLength(2);
						return TypeFactory.GenericMap(Role, ReferencedPropertyName, GenericArguments[0], GenericArguments[1]);
					}
				}

				// No Generic behavior
				if (IsSorted)
				{
					return TypeFactory.SortedMap(Role, ReferencedPropertyName, Embedded, (IComparer)Comparer);
				}
				else if (HasOrder)
				{
					return TypeFactory.OrderedMap(Role, ReferencedPropertyName, Embedded);
				}
				else
				{
					return TypeFactory.Map(Role, ReferencedPropertyName, Embedded);
				}
			}
		}

		public override void CreateAllKeys()
		{
			base.CreateAllKeys();
			if (!IsInverse)
				Index.CreateForeignKey();
		}
	}
}
