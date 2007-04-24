using System;
using System.Text;

namespace NHibernate.Mapping
{
	public class SingleTableSubclass : Subclass
	{
		public SingleTableSubclass(PersistentClass superclass)
			: base(superclass) { }

		public override void Validate(NHibernate.Engine.IMapping mapping)
		{
			if (Discriminator == null)
			{
				throw new MappingException(string.Format("No discriminator found for {0}. Discriminator is needed when 'single-table-per-hierarchy' is used and a class has subclasses", MappedClass.Name));
			}
			base.Validate(mapping);
		}
	}
}