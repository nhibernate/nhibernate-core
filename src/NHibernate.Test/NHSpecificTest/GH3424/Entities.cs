using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3424
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<Child> Children { get; set; }
	}

	class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
