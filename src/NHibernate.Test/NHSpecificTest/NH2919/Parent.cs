using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2919
{
	public class Parent
	{
		public virtual Guid ID { get; set; }

		public virtual string Name { get; set; }

		public virtual ParentSummary Summary { get; set; }

		public virtual ISet<Child> Children { get; set; }
	}
}
