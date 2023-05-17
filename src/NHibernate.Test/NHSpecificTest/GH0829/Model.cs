using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH0829
{
	public class Parent
	{
		public virtual Guid Id { get; set; }

		public virtual TestEnum Type { get; set; }

		public virtual IList<Child> Children { get; set; } = new List<Child>();
	}

	public class Child
	{
		public virtual Guid Id { get; set; }

		public virtual TestEnum Type { get; set; }

		public virtual Parent Parent { get; set; }
	}
}
