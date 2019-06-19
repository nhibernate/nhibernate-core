using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH2089
{
	public class Parent
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual IList<Child> Children { get; protected set; } = new List<Child>();

		public virtual void AddChild(Child child)
		{
			Children.Add(child);
			child.Parent = this;
		}
		
		public override bool Equals(object obj)
		{
			return obj is Parent other && Id == other.Id;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}

	public class Child
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Parent Parent { get; set; }
		
		public virtual int OrderIndex
		{
			get
			{
				if (Parent == null)
					return -2;
				return Parent.Children.IndexOf(this);
			}
		}
		
		public override bool Equals(object obj)
		{
			return obj is Child other && Id == other.Id;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}
}
