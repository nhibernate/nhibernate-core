using System;
using System.Collections.Generic;
using NHibernate.Hql.Ast;

namespace NHibernate.Linq
{
    public class HqlNodeStack
    {
        private readonly Stack<HqlTreeNode> _stack = new Stack<HqlTreeNode>();
        private readonly HqlNill _root;

        public HqlNodeStack(HqlTreeBuilder builder)
        {
            // TODO - only reason for the build is to have a root node.  Sucks, change this
            _root = builder.Holder();
            _stack.Push(_root);
        }

        public IEnumerable<HqlTreeNode> NodesPreOrder
        {
            get { return _root.NodesPreOrder; }
        }

        public IEnumerable<HqlTreeNode> Finish()
        {
            var holder = (HqlNill) _stack.Pop();

            return holder.Children;
        }

        public void PushLeaf(HqlTreeNode query)
        {
            PushNode(query).Dispose();
        }

        public IDisposable PushNode(HqlTreeNode query)
        {
            _stack.Peek().AddChild(query);

            _stack.Push(query);

            var stackEntry = new HqlNodeStackEntry(this, query);

            return stackEntry;
        }

        private HqlTreeNode Peek()
        {
            return _stack.Peek();
        }

        private void Pop()
        {
            _stack.Pop();
        }

        public class HqlNodeStackEntry : IDisposable
        {
            private readonly HqlNodeStack _parent;
            private readonly HqlTreeNode _node;

            internal HqlNodeStackEntry(HqlNodeStack parent, HqlTreeNode node)
            {
                _parent = parent;
                _node = node;
            }

            public void Dispose()
            {
                if (_parent.Peek() != _node)
                {
                    throw new InvalidOperationException();
                }

                _parent.Pop();
            }
        }
    }
}