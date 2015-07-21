using System;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	[Serializable]
	public class SingleTableSubclass : Subclass
	{
		public SingleTableSubclass(PersistentClass superclass)
			: base(superclass) { }

		protected internal override IEnumerable<Property> NonDuplicatedPropertyIterator
		{
			get { return new JoinedEnumerable<Property>(Superclass.UnjoinedPropertyIterator, UnjoinedPropertyIterator); }
		}

		protected internal override IEnumerable<ISelectable> DiscriminatorColumnIterator
		{
			get
			{
				if (IsDiscriminatorInsertable && !Discriminator.HasFormula)
				{
					return Discriminator.ColumnIterator;
				}
				else
				{
					return base.DiscriminatorColumnIterator;
				}
			}
		}

		public override void Validate(Engine.IMapping mapping)
		{
			if (Discriminator == null)
			{
				throw new MappingException("No discriminator found for " + EntityName + ". Discriminator is needed when 'single-table-per-hierarchy' is used and a class has subclasses");
			}
			base.Validate(mapping);
		}
	}
}