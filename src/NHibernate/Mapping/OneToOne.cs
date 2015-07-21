using System;
using System.Collections.Generic;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A mapping for a <c>one-to-one</c> association.
	/// </summary>
	[Serializable]
	public class OneToOne : ToOne
	{
		private bool constrained;
		private ForeignKeyDirection foreignKeyType;
		private IKeyValue identifier;
		private string propertyName;
		private string entityName;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		/// <param name="owner"></param>
		public OneToOne(Table table, PersistentClass owner)
			: base(table)
		{
			identifier = owner.Key;
			entityName = owner.EntityName;
		}

		/// <summary></summary>
		public override void CreateForeignKey()
		{
			if (constrained && referencedPropertyName == null)
			{
				//TODO: handle the case of a foreign key to something other than the pk
				CreateForeignKeyOfEntity(((EntityType)Type).GetAssociatedEntityName());
			}
		}

		/// <summary></summary>
		public override IEnumerable<Column> ConstraintColumns
		{
			get { return new SafetyEnumerable<Column>(identifier.ColumnIterator); }
		}

		/// <summary></summary>
		public bool IsConstrained
		{
			get { return constrained; }
			set { constrained = value; }
		}

		/// <summary></summary>
		public ForeignKeyDirection ForeignKeyType
		{
			get { return foreignKeyType; }
			set { foreignKeyType = value; }
		}

		/// <summary></summary>
		public IKeyValue Identifier
		{
			get { return identifier; }
			set { identifier = value; }
		}

		/// <summary></summary>
		public override bool IsNullable
		{
			get { return !constrained; }
		}

		public string EntityName
		{
			get { return entityName; }
			set { entityName = StringHelper.InternedIfPossible(value); }
		}

		public string PropertyName
		{
			get { return propertyName; }
			set { propertyName = StringHelper.InternedIfPossible(value); }
		}

		public override IType Type
		{
			get
			{
				if (ColumnSpan > 0)
				{
					return
						new SpecialOneToOneType(ReferencedEntityName, foreignKeyType, referencedPropertyName, IsLazy, UnwrapProxy,
												entityName, propertyName);
				}
				else
				{
					return
						TypeFactory.OneToOne(ReferencedEntityName, foreignKeyType, referencedPropertyName, IsLazy, UnwrapProxy, Embedded,
											 entityName, propertyName);
				}
			}
		}
	}
}