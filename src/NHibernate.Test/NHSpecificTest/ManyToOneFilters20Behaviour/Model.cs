using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.ManyToOneFilters20Behaviour
{
	public class Parent
	{
		public virtual Guid Id { get; set; }
		public virtual Child Child { get; set; }
		public virtual Address Address { get; set; }
		public virtual IList<Child> Children { get; set; }
		public virtual string ParentString { get; set; }
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
		public virtual string ChildString { get; set; }
	}

	public class Address
	{
		public virtual Guid Id { get; set; }
		public virtual Parent Parent { get; set; }
		public virtual bool IsActive { get; set; }
	}
}
