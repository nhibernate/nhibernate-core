using System;
using Antlr.Runtime;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class InLogicOperatorNode : BinaryLogicOperatorNode, IBinaryOperatorNode
	{
		public InLogicOperatorNode(IToken token)
			: base(token)
		{
		}

		private IASTNode InList
		{
			get { return RightHandOperand; }
		}

		public override void Initialize()
		{
			IASTNode lhs = LeftHandOperand;
			if (lhs == null)
			{
				throw new SemanticException("left-hand operand of in operator was null");
			}

			IASTNode inList = InList;
			if (inList == null)
			{
				throw new SemanticException("right-hand operand of in operator was null");
			}

			// for expected parameter type injection, we expect that the lhs represents
			// some form of property ref and that the children of the in-list represent
			// one-or-more params.
			var lhsNode = lhs as SqlNode;
			if (lhsNode != null)
			{
				IType lhsType = lhsNode.DataType;
				IASTNode inListChild = inList.GetChild(0);
				while (inListChild != null)
				{
					var expectedTypeAwareNode = inListChild as IExpectedTypeAwareNode;
					if (expectedTypeAwareNode != null)
					{
						expectedTypeAwareNode.ExpectedType = lhsType;
					}
					inListChild = inListChild.NextSibling;
				}
			}
		}
	}
}
