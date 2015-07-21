using System.Collections.Generic;


namespace NHibernate.Test.Stateless
{
	public class TreeNode
	{
		private ISet<TreeNode> _children = new HashSet<TreeNode>();

		public virtual int Id { get; protected set; }

		public virtual string Content { get; set; }

		public virtual ISet<TreeNode> Children
		{
			get { return _children; }
			protected set { _children = value; }
		}
	}
}
