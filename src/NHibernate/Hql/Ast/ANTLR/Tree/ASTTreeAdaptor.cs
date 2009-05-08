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

		public override object GetParent(object t)
		{
			throw new System.NotImplementedException();
		}

		public override void SetParent(object t, object parent)
		{
			((ITree)t).Parent = (ITree)parent;
		}

		public override int GetChildIndex(object t)
		{
			return ((ITree)t).ChildIndex;
		}

		public override void SetChildIndex(object t, int index)
		{
			((ITree)t).ChildIndex = index;
		}

		public override int GetNodeType(object t)
		{
			return ((ITree) t).Type;
		}

		public override void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t)
		{
			throw new System.NotImplementedException();
		}

		public override void SetTokenBoundaries(object t, IToken startToken, IToken stopToken)
		{
			if (t != null)
			{
				int tokenIndex = 0;
				int num2 = 0;
				if (startToken != null)
				{
					tokenIndex = startToken.TokenIndex;
				}
				if (stopToken != null)
				{
					num2 = stopToken.TokenIndex;
				}
				((ITree)t).TokenStartIndex = tokenIndex;
				((ITree)t).TokenStopIndex = num2;
			}
		}

		public override int GetTokenStartIndex(object t)
		{
			throw new System.NotImplementedException();
		}

		public override int GetTokenStopIndex(object t)
		{
			throw new System.NotImplementedException();
		}
	}
}
