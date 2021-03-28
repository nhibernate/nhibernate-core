using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH1738
{
	class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Child> Children { get; set; } = new List<Child>();
	}
	
	class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Entity Parent { get; set; }
	}
}
