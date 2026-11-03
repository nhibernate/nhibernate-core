using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3863
{
	public class Parent
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual ISet<Child> Children { get; set; } = new HashSet<Child>();
	}

	public class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Parent Parent { get; set; }
	}
}
