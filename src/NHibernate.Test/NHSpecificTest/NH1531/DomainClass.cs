using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1531
{
	public class Parent
	{
		private readonly IList<Child> _children = new List<Child>();

		public int Id { get; set; }

		public IList<Child> Children
		{
			get { return _children; }
		}

		public void AddNewChild()
		{
			var c = new Child {Name = "New Child", Parent = this};
			_children.Add(c);
		}

		public void DetachAllChildren()
		{
			foreach (var c in _children)
			{
				c.Parent = null;
			}
			_children.Clear();
		}

		public void AttachNewChild(Child c)
		{
			if (c.Parent != null)
			{
				c.Parent.Children.Remove(c);
			}
			c.Parent = this;
			_children.Add(c);
		}
	}

	public class Child
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public Parent Parent { get; set; }
	}
}