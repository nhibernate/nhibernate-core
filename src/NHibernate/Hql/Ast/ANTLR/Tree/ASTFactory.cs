using Antlr.Runtime.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	public class ASTFactory : IASTFactory
	{
		private readonly ITreeAdaptor _adaptor;

		public ASTFactory(ITreeAdaptor adaptor)
		{
			_adaptor = adaptor;
		}

		public IASTNode CreateNode(int type, string text, params IASTNode[] children)
		{
			IASTNode parent = (IASTNode)_adaptor.Create(type, text);

			parent.AddChildren(children);

			return parent;
		}
	}
}
