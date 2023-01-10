using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3218
{
	public class Entity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Child> List { get; set; }
	}

	public class Child
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
