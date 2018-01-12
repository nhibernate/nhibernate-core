using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.CollectionPerf
{
	public class Parent
	{
		public virtual Guid Id { get; set; }
		public virtual IList<Child> Children { get; set; } = new List<Child>();
		public virtual string Name { get; set; } = Guid.NewGuid().ToString();

		public virtual void AddChild(Child child)
		{
			Children.Add(child);
			child.Parent = this;
		}

		public virtual Parent MakeCopy()
		{
			var ret = new Parent{Id=Id, Name=Name};
			foreach (var child in Children)
			{
				ret.AddChild(new Child{Id=child.Id, Name = child.Name});
			}
			return ret;
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
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; } = Guid.NewGuid().ToString();
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
