using System;

namespace NHibernate.Test.NHSpecificTest.ManyToOneFilters20Behaviour
{
	public class Parent
	{
		public virtual Guid Id { get; set; }
		public virtual Child Child { get; set; }
	}

	public class Child
	{
		public Child()
		{
			Always = true;
		}

		public virtual Guid Id { get; set; }
		public virtual bool IsActive { get; set; }
		public virtual bool Always { get; set; }
	}
}
