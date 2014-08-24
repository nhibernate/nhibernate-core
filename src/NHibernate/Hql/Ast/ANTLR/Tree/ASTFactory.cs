using System;
using Antlr.Runtime.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class ASTFactory : IASTFactory
	{
		private readonly ITreeAdaptor _adaptor;

		public ASTFactory(ITreeAdaptor adaptor)
		{
			_adaptor = adaptor;
		}

		#region IASTFactory Members

		public IASTNode CreateNode(int type, string text, params IASTNode[] children)
		{
			var parent = (IASTNode) _adaptor.Create(type, text);

			parent.AddChildren(children);

			return parent;
		}

		#endregion
	}
}