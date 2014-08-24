using System;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class ASTTreeAdaptor : BaseTreeAdaptor
	{
		public override object DupNode(object t)
		{
			if (t == null)
			{
				return null;
			}
			return ((IASTNode)t).DupNode();
		}

		public override object Create(IToken payload)
		{
			return new ASTNode(payload);
		}

		public override IToken GetToken(object treeNode)
		{
			return ((ASTNode) treeNode).Token;
		}

		public override IToken CreateToken(int tokenType, string text)
		{
			return new CommonToken(tokenType, text);
		}

		public override IToken CreateToken(IToken fromToken)
		{
			return new CommonToken(fromToken);
		}

		public override object ErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e)
		{
			return new ASTErrorNode(input, start, stop, e);
		}
	}
}
