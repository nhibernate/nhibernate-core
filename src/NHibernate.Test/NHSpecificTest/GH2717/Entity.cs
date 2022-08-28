using System;

namespace NHibernate.Test.NHSpecificTest.GH2717
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	class Subclass : Entity
	{
	}
}
