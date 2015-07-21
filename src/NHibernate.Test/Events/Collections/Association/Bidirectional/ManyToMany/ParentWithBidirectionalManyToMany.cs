using System.Collections.Generic;

namespace NHibernate.Test.Events.Collections.Association.Bidirectional.ManyToMany
{
	public class ParentWithBidirectionalManyToMany : AbstractParentWithCollection
	{
		public ParentWithBidirectionalManyToMany() {}

		public ParentWithBidirectionalManyToMany(string name) : base(name) {}

		public override IChild CreateChild(string name)
		{
			return new ChildWithBidirectionalManyToMany(name, new HashSet<ParentWithBidirectionalManyToMany>());
		}

		public override void NewChildren(ICollection<IChild> children)
		{
			if (children == Children)
			{
				return;
			}
			if (Children != null)
			{
				foreach (ChildWithBidirectionalManyToMany child in Children)
				{
					child.RemoveParent(this);
				}
			}
			if (children != null)
			{
				foreach (ChildWithBidirectionalManyToMany child in children)
				{
					child.AddParent(this);
				}
			}
			base.NewChildren(children);
		}

		public override void AddChild(IChild child)
		{
			base.AddChild(child);
			((ChildWithBidirectionalManyToMany) child).AddParent(this);
		}

		public override void AddAllChildren(ICollection<IChild> children)
		{
			base.AddAllChildren(children);
			foreach (ChildWithBidirectionalManyToMany child in children)
			{
				child.AddParent(this);
			}
		}

		public override void RemoveChild(IChild child)
		{
			// Note: if the collection is a bag, the same child can be in the collection more than once
			base.RemoveChild(child);
			// only remove the parent from the child's set if child is no longer in the collection
			if (!Children.Contains(child))
			{
				((ChildWithBidirectionalManyToMany) child).RemoveParent(this);
			}
		}

		public override void RemoveAllChildren(ICollection<IChild> children)
		{
			base.RemoveAllChildren(children);
			foreach (ChildWithBidirectionalManyToMany child in children)
			{
				child.RemoveParent(this);
			}
		}

		public override void ClearChildren()
		{
			if (Children != null)
			{
				foreach (ChildWithBidirectionalManyToMany child in Children)
				{
					child.RemoveParent(this);
				}
			}
			base.ClearChildren();
		}
	}
}