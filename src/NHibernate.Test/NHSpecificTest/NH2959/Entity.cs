using System;

namespace NHibernate.Test.NHSpecificTest.NH2959
{
	abstract class BaseEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	class DerivedEntity : BaseEntity
	{ }

	class AnotherDerivedEntity : BaseEntity
	{ }
}