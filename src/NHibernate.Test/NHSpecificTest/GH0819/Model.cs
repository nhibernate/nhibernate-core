using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH0819
{
	class Parent
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		public virtual IList<Child> Children { get; set; }
	}

	class Child
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }
		
		public virtual Parent Parent { get; set; }
	}
}
