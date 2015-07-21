using System;
using System.Collections.Generic;

namespace NHibernate.Test.Operations
{
	public class NumberedNode
	{
		private readonly ISet<NumberedNode> children = new HashSet<NumberedNode>();

		protected NumberedNode() {}

		public NumberedNode(string name)
		{
			Name = name;
			Created = DateTime.Now;
		}

		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
		public virtual NumberedNode Parent { get; set; }

		public virtual ICollection<NumberedNode> Children
		{
			get { return children; }
		}

		public virtual string Description { get; set; }
		public virtual DateTime Created { get; set; }

		public virtual NumberedNode AddChild(NumberedNode child)
		{
			children.Add(child);
			child.Parent = this;
			return this;
		}
	}
}