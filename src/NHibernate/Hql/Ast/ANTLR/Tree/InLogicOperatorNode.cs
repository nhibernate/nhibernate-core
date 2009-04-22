using Antlr.Runtime;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	public class InLogicOperatorNode : BinaryLogicOperatorNode, IBinaryOperatorNode
	{
		public InLogicOperatorNode(IToken token)
			: base(token)
		{
		}

		public IASTNode getInList()
		{
			return RightHandOperand;
		}

		public override void Initialize()
		{
			IASTNode lhs = LeftHandOperand;
			if (lhs == null)
			{
				throw new SemanticException("left-hand operand of in operator was null");
			}

			IASTNode inList = getInList();
			if (inList == null)
			{
				throw new SemanticException("right-hand operand of in operator was null");
			}

			// for expected parameter type injection, we expect that the lhs represents
			// some form of property ref and that the children of the in-list represent
			// one-or-more params.
			if (typeof(SqlNode).IsAssignableFrom(lhs.GetType()))
			{
				IType lhsType = ((SqlNode)lhs).DataType;
				IASTNode inListChild = inList.GetChild(0);
				while (inListChild != null)
				{
					if (typeof(IExpectedTypeAwareNode).IsAssignableFrom(inListChild.GetType()))
					{
						((IExpectedTypeAwareNode)inListChild).ExpectedType = lhsType;
					}
					inListChild = inListChild.NextSibling;
				}
			}
		}
	}
}
