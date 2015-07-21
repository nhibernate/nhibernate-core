using System;
using System.Collections.Generic;

namespace NHibernate.Test.ReadOnly
{
	public class VersionedNode
	{
		private string id;
		private string name;
		private long version;
		private VersionedNode parent;
		private ISet<VersionedNode> children = new HashSet<VersionedNode>();

		public VersionedNode()
		{
		}
	
		public VersionedNode(string id, string name)
		{
			this.id = id;
			this.name = name;
		}
	
		public virtual string Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	
		public virtual long Version
		{
			get { return version; }
			set { version = value; }
		}
		
		public virtual VersionedNode Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public virtual ISet<VersionedNode> Children
		{
			get { return children; }
			set { children = value; }
		}
	
		public virtual void AddChild(VersionedNode child)
		{
			child.Parent = this;
			children.Add(child);
		}
	}
}
