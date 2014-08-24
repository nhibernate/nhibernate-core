using System.Collections.Generic;
using NHibernate.Type;
using NHibernate.Util;
using System;

namespace NHibernate.Mapping
{
	/// <summary> A many-to-one association mapping</summary>
	[Serializable]
	public class ManyToOne : ToOne
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		public ManyToOne(Table table) : base(table)
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

		public bool IsIgnoreNotFound
		{
			get { return isIgnoreNotFound; }
			set { isIgnoreNotFound = value; }
		}

		private IType type;
		public override IType Type
		{
			get
			{
				if (type == null)
				{
					type =
						TypeFactory.ManyToOne(ReferencedEntityName, ReferencedPropertyName, IsLazy, UnwrapProxy, Embedded, IsIgnoreNotFound);
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

				if (!HasFormula && !"none".Equals(ForeignKeyName, StringComparison.InvariantCultureIgnoreCase))
				{

					IEnumerable<Column> ce = new SafetyEnumerable<Column>(property.ColumnIterator);

					// NH : The four lines below was added to ensure that related columns have same length,
					// like ForeignKey.AlignColumns() do
					IEnumerator<Column> fkCols = ConstraintColumns.GetEnumerator();
					IEnumerator<Column> pkCols = ce.GetEnumerator();
					while (fkCols.MoveNext() && pkCols.MoveNext())
						fkCols.Current.Length = pkCols.Current.Length;

					ForeignKey fk =
						Table.CreateForeignKey(ForeignKeyName, ConstraintColumns, ((EntityType) Type).GetAssociatedEntityName(), ce);
					fk.CascadeDeleteEnabled = IsCascadeDeleteEnabled;
				}
			}
		}
	}
}