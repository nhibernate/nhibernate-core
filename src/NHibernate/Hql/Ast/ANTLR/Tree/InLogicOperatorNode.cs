using System;
using System.Collections.Generic;
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
			IType lhsType = null;
			if (lhsNode != null)
			{
				lhsType = lhsNode.DataType;
				IASTNode inListChild = inList.GetChild(0);
				while (inListChild != null)
				{
					if (inListChild is IExpectedTypeAwareNode expectedTypeAwareNode &&
						expectedTypeAwareNode.ExpectedType == null)
					{
						expectedTypeAwareNode.ExpectedType = lhsType;
					}

					inListChild = inListChild.NextSibling;
				}
			}

			var sessionFactory = SessionFactoryHelper.Factory;
			if (sessionFactory.Dialect.SupportsRowValueConstructorSyntaxInInList)
				return;

			lhsType = lhsType ?? ExtractDataType(lhs);
			if (lhsType == null)
				return;

			var rhsNode = inList.GetFirstChild();
			if (rhsNode == null || !IsNodeAcceptable(rhsNode))
				return;

			var lhsColumnSpan = lhsType.GetColumnSpan(sessionFactory);
			var rhsColumnSpan = rhsNode.Type == HqlSqlWalker.VECTOR_EXPR
				? rhsNode.ChildCount
				: ExtractDataType(rhsNode)?.GetColumnSpan(sessionFactory) ?? 0;

			if (lhsColumnSpan > 1 && rhsColumnSpan > 1)
			{
				MutateRowValueConstructorSyntaxInInListSyntax(lhs, lhsColumnSpan, rhsNode, rhsColumnSpan);
			}
		}

		/// <summary>
		/// this is possible for parameter lists and explicit lists. It is completely unreasonable for sub-queries.
		/// </summary>
		private static bool IsNodeAcceptable(IASTNode rhsNode)
		{
			return rhsNode == null /* empty IN list */
				|| rhsNode is LiteralNode
				|| rhsNode is ParameterNode
				|| rhsNode.Type == HqlSqlWalker.VECTOR_EXPR;
		}

		/// <summary>
		///  Mutate the subtree relating to a row-value-constructor in "in" list to instead use
		/// a series of ORen and ANDed predicates.  This allows multi-column type comparisons
		/// and explicit row-value-constructor in "in" list syntax even on databases which do
		/// not support row-value-constructor in "in" list.
		/// 
		/// For example, here we'd mutate "... where (col1, col2) in ( ('val1', 'val2'), ('val3', 'val4') ) ..." to
		/// "... where (col1 = 'val1' and col2 = 'val2') or (col1 = 'val3' and val2 = 'val4') ..."
		/// </summary>
		private void MutateRowValueConstructorSyntaxInInListSyntax(IASTNode lhsNode, int lhsColumnSpan, IASTNode rhsNode, int rhsColumnSpan)
		{
			//NHibenate specific: In hibernate they recreate new tree in HQL. In NHibernate we just replace node with generated SQL
			// (same as it's done in BinaryLogicOperatorNode)

			string[] lhsElementTexts = ExtractMutationTexts(lhsNode, lhsColumnSpan);

			if (lhsNode is ParameterNode lhsParam && lhsParam.HqlParameterSpecification != null)
			{
				AddEmbeddedParameter(lhsParam.HqlParameterSpecification);
			}

			var negated = Type == HqlSqlWalker.NOT_IN;

			var andElementsNodeList = new List<string>();

			while (rhsNode != null)
			{
				string[] rhsElementTexts = ExtractMutationTexts(rhsNode, rhsColumnSpan);
				if (rhsNode is ParameterNode rhsParam && rhsParam.HqlParameterSpecification != null)
				{
					AddEmbeddedParameter(rhsParam.HqlParameterSpecification);
				}

				var text = Translate(lhsColumnSpan, "=", lhsElementTexts, rhsElementTexts);

				andElementsNodeList.Add(negated ? string.Concat("( not ", text, ")") : text);
				rhsNode = rhsNode.NextSibling;
			}

			ClearChildren();
			Type = HqlSqlWalker.SQL_TOKEN;
			var sqlToken = string.Join(negated ? " and " : " or ", andElementsNodeList);
			Text = andElementsNodeList.Count > 1 ? string.Concat("(", sqlToken, ")") : sqlToken;
		}
	}
}
