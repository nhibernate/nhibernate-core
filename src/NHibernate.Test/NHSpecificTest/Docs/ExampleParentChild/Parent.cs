using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.Docs.ExampleParentChild
{
	public class Parent
	{
		private long _id;
		private ISet<Child> _children;

		public Parent()
		{
		}

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public ISet<Child> Children
		{
			get { return _children; }
			set { _children = value; }
		}

		public void AddChild(Child c)
		{
			if (Children == null)
			{
				Children = new HashSet<Child>();
			}
			Children.Add(c);
			c.Parent = this;
		}
	}
}