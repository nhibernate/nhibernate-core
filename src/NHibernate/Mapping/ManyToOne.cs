using System.Collections.Generic;
using NHibernate.Type;
using System;
using System.Linq;

namespace NHibernate.Mapping
{
	/// <summary> A many-to-one association mapping</summary>
	[Serializable]
	public class ManyToOne : ToOne
	{
		private readonly string _entityName;

		/// <summary>
		/// Construct a many-to-one mapping.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="owner">The association owner.</param>
		public ManyToOne(Table table, PersistentClass owner) : base(table)
		{
			_entityName = owner?.EntityName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		[Obsolete("Use overload with owner instead.")]
		public ManyToOne(Table table) : this(table, null)
		{
		}

		/// <summary></summary>
		public override void CreateForeignKey()
		{
			// the case of a foreign key to something other than the pk is handled in createPropertyRefConstraints
			if (ReferencedPropertyName == null && !HasFormula && !IsIgnoreNotFound)
			{
				CreateForeignKeyOfEntity(((EntityType)Type).GetAssociatedEntityName());
			}
		}

		private bool isIgnoreNotFound = false;
		private bool isLogicalOneToOne;

		public bool IsIgnoreNotFound
		{
			get { return isIgnoreNotFound; }
			set { isIgnoreNotFound = value; }
		}

		public bool IsLogicalOneToOne
		{
			get { return isLogicalOneToOne; }
			set { isLogicalOneToOne = value; }
		}

		public string PropertyName { get; set; }

		private IType type;
		public override IType Type
		{
			get
			{
				if (type == null)
				{
					type = TypeFactory.ManyToOne(
						ReferencedEntityName, ReferencedPropertyName, IsLazy, UnwrapProxy,
						IsIgnoreNotFound, isLogicalOneToOne, _entityName, PropertyName);
				}
				return type;
			}
		}

		public void CreatePropertyRefConstraints(IDictionary<string, PersistentClass> persistentClasses)
		{
			if (!string.IsNullOrEmpty(referencedPropertyName))
			{
				if (string.IsNullOrEmpty(ReferencedEntityName))
					throw new MappingException(
						string.Format("ReferencedEntityName not specified for property '{0}' on entity {1}", ReferencedPropertyName, this));

				PersistentClass pc;
				persistentClasses.TryGetValue(ReferencedEntityName, out pc);
				if (pc == null)
					throw new MappingException(string.Format("Could not find referenced entity '{0}' on {1}", ReferencedEntityName, this));

				Property property = pc.GetReferencedProperty(ReferencedPropertyName);
				if (property == null)
					throw new MappingException("Could not find property " + ReferencedPropertyName + " on " + ReferencedEntityName);

				if (!HasFormula && !"none".Equals(ForeignKeyName, StringComparison.OrdinalIgnoreCase))
				{
					IEnumerable<Column> ce = property.ColumnIterator.OfType<Column>();

					// NH : Ensure that related columns have same length
					ForeignKey.AlignColumns(ConstraintColumns, ce);

					ForeignKey fk =
						Table.CreateForeignKey(ForeignKeyName, ConstraintColumns, ((EntityType)Type).GetAssociatedEntityName(), ce);
					fk.CascadeDeleteEnabled = IsCascadeDeleteEnabled;
				}
			}
		}
	}
}
