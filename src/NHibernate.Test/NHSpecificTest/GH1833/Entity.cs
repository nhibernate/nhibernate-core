using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1833
{
	class Entity
	{
		public virtual string Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<Child> Children { get; set; }
	}

	class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string ParentName { get; set; }
	}
}
