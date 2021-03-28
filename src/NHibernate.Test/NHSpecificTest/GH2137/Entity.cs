using System;

namespace NHibernate.Test.NHSpecificTest.GH2137
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ExtendedProperties ExtendedProperties { get; set; }
	}

	class ExtendedProperties
	{
		public virtual Guid Id { get; set; }

		public virtual string Value { get; set; }
	}
}
