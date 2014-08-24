using System.Collections.Generic;

namespace NHibernate.Test.Events.Collections.Association.Bidirectional.OneToMany
{
	public class ParentWithBidirectionalOneToMany : AbstractParentWithCollection
	{
		public ParentWithBidirectionalOneToMany() { }

		public ParentWithBidirectionalOneToMany(string name) : base(name) { }

		public override IChild CreateChild(string name)
		{
			return new ChildWithManyToOne(name);
		}

		public override void AddChild(IChild child)
		{
			base.AddChild(child);
			((ChildWithManyToOne)child).Parent = this;
		}

		public override void NewChildren(ICollection<IChild> children)
		{
			if (children == Children)
			{
				return;
			}
			if (Children != null)
			{
				foreach (ChildWithManyToOne child in Children)
				{
					child.Parent = null;
				}
			}
			if (children != null)
			{
				foreach (ChildWithManyToOne child in children)
				{
					child.Parent = this;
				}
			}
			base.NewChildren(children);
		}

		public override void RemoveChild(IChild child)
		{
			// Note: there can be more than one child in the collection
			base.RemoveChild(child);
			// only set the parent to null if child is no longer in the bag
			if (!Children.Contains(child))
			{
				((ChildWithManyToOne)child).Parent = null;

			}
		}
	}
}