using Antlr.Runtime.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
    public class HqlSqlWalkerTreeNodeStream : CommonTreeNodeStream
    {
        public HqlSqlWalkerTreeNodeStream(object tree)
            : base(tree)
        {
            
        }
        public HqlSqlWalkerTreeNodeStream(ITreeAdaptor adaptor, object tree)
            : base(adaptor, tree)
        {
            
        }
        public HqlSqlWalkerTreeNodeStream(ITreeAdaptor adaptor, object tree, int initialBufferSize)
            : base(adaptor, tree, initialBufferSize)
        {
            
        }

        public void InsertChild(IASTNode parent, IASTNode child)
        {
            // Adding a child to the current node. If currently no children, then also need to insert Down & Up nodes
            bool needUp = false;
            int insertPoint = nodes.IndexOf(parent) + parent.ChildCount + 1;

            if (parent.ChildCount == 0)
            {
                nodes.Insert(insertPoint, down);
                needUp = true;
            }
            insertPoint++; // We either just inserted a Down node, or one existed already which we need to count

            parent.AddChild(child);
            nodes.Insert(insertPoint, child);
            insertPoint++;

            if (needUp)
            {
                nodes.Insert(insertPoint, up);
            }
        }
    }
}
