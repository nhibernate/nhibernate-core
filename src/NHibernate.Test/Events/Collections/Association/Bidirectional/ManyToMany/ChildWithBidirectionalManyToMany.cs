using System.Collections.Generic;

namespace NHibernate.Test.Events.Collections.Association.Bidirectional.ManyToMany
{
	public class ChildWithBidirectionalManyToMany : ChildEntity
	{
		private ICollection<ParentWithBidirectionalManyToMany> parents;
		public ChildWithBidirectionalManyToMany() {}

		public ChildWithBidirectionalManyToMany(string name, ICollection<ParentWithBidirectionalManyToMany> parents)
			: base(name)
		{
			this.parents = parents;
		}

		public virtual ICollection<ParentWithBidirectionalManyToMany> Parents
		{
			get { return parents; }
			set { parents = value; }
		}

		public virtual void AddParent(ParentWithBidirectionalManyToMany parent)
		{
			if (parent != null)
			{
				parents.Add(parent);
			}
		}

		public virtual void RemoveParent(ParentWithBidirectionalManyToMany parent)
		{
			if (parent != null)
			{
				parents.Remove(parent);
			}
		}
	}
}