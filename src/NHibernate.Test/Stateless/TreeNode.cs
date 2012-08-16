using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iesi.Collections.Generic;

namespace NHibernate.Test.Stateless
{
    public class TreeNode
    {
        private ISet<TreeNode> children = new HashedSet<TreeNode>();

        public virtual int Id { get; protected set; }

        public virtual string Content { get; set; }

        public virtual ISet<TreeNode> Children
        {
            get { return children; }
            protected set { children = value; }
        }
    }
}
