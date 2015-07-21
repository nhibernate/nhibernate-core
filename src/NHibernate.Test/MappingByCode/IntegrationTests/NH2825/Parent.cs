using System;
using System.Collections.Generic;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH2825
{
	public class Parent
	{
		public Parent()
		{
			Children = new List<Child>();
		}
		public virtual Guid Id { get; set; }
		public virtual int ParentCode { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Child> Children { get; set; }

		public virtual void AddChild(Child child)
		{
			Children.Add(child);
			child.Parent = this;
		}
	}
}