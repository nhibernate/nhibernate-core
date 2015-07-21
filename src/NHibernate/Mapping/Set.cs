using System;
using System.Collections;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A Set with no nullable element columns will have a primary
	/// key consisting of all table columns (ie - key columns + 
	/// element columns).
	/// </summary>
	[Serializable]
	public class Set : Collection
	{
		public Set(PersistentClass owner) : base(owner)
		{
		}

		public override bool IsSet
		{
			get { return true; }
		}

		public override CollectionType DefaultCollectionType
		{
			get
			{
				System.Type elementType = typeof (object);

				// If this set is part of a dynamic component, IsGeneric will be false, in
				// which case we default to typing as object.
				// TODO: For sets in dynamic component, grab the element type from the class attribute in the mappings.

				if (this.IsGeneric)
				{
					CheckGenericArgumentsLength(1);
					elementType = this.GenericArguments[0];
					//throw new NotSupportedException("Only generic sets are supported.");
				}

				if (IsSorted)
				{
					return TypeFactory.GenericSortedSet(Role, ReferencedPropertyName, Comparer, elementType);
				}
				else if (HasOrder)
				{
					return TypeFactory.GenericOrderedSet(Role, ReferencedPropertyName, elementType);
				}
				else
				{
					return TypeFactory.GenericSet(Role, ReferencedPropertyName, elementType);
				}
			}
		}

		public override void CreatePrimaryKey()
		{
			if (!IsOneToMany)
			{
				PrimaryKey pk = new PrimaryKey();
				foreach (ISelectable selectable in Key.ColumnIterator)
				{
					if (!selectable.IsFormula)
					{
						Column col = (Column)selectable;
						pk.AddColumn(col);
					}
				}

				bool nullable = false;
				foreach (ISelectable selectable in Element.ColumnIterator)
				{
					if (!selectable.IsFormula)
					{
						Column col = (Column) selectable;
						if (col.IsNullable)
						{
							nullable = true;
						}
						pk.AddColumn(col);
					}
				}

				// some databases (Postgres) will tolerate nullable
				// column in a primary key - others (DB2) won't
				if (!nullable)
				{
					CollectionTable.PrimaryKey = pk;
				}
			}
			else
			{
				// Create an index on the key columns?
			}
		}
	}
}
