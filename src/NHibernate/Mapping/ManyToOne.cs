using System.Collections.Generic;
using NHibernate.Type;
using NHibernate.Util;
using System;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class ManyToOne : ToOne
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		public ManyToOne(Table table) : base(table)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyClass"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyAccess"></param>
		public override void SetTypeByReflection(System.Type propertyClass, string propertyName, string propertyAccess)
		{
			try
			{
				if (Type == null)
				{
					System.Type refClass = ReflectHelper.ReflectedPropertyClass(propertyClass, propertyName, propertyAccess);
					ReferencedEntityName = refClass.FullName;
					Type = TypeFactory.ManyToOne(refClass, ReferencedPropertyName, IsLazy, isIgnoreNotFound);
				}
			}
			catch (HibernateException he)
			{
				throw new MappingException("Problem trying to set association type by reflection", he);
			}
		}

		/// <summary></summary>
		public override void CreateForeignKey()
		{
			// the case of a foreign key to something other than the pk is handled in createPropertyRefConstraints
			if (referencedPropertyName == null && !HasFormula && !isIgnoreNotFound)
			{
				CreateForeignKeyOfEntity(((EntityType)Type).GetAssociatedEntityName());
			}
		}

		private bool isIgnoreNotFound = false; // NH-268

		public bool IsIgnoreNotFound
		{
			get { return isIgnoreNotFound; }
			set { isIgnoreNotFound = value; }
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