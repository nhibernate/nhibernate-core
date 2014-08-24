using System;
using Antlr.Runtime;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Contract for nodes representing logcial BETWEEN (ternary) operators.
	/// </summary>
	[CLSCompliant(false)]
	public class BetweenOperatorNode : SqlNode, IOperatorNode
	{
		public BetweenOperatorNode(IToken token)
			: base(token)
		{
		}

		public void Initialize()
		{
			IASTNode fixture = GetFixtureOperand();
			if (fixture == null)
			{
				throw new SemanticException("fixture operand of a between operator was null");
			}

			IASTNode low = GetLowOperand();
			if (low == null)
			{
				throw new SemanticException("low operand of a between operator was null");
			}

			IASTNode high = GetHighOperand();
			if (high == null)
			{
				throw new SemanticException("high operand of a between operator was null");
			}

			Check(fixture, low, high);
			Check(low, high, fixture);
			Check(high, fixture, low);
		}

		public override IType DataType
		{
			get { return NHibernateUtil.Boolean; }
			set { base.DataType = value; }
		}

		private IASTNode GetFixtureOperand()
		{
			return GetChild(0);
		}

		private IASTNode GetLowOperand()
		{
			return GetChild(1);
		}

		private IASTNode GetHighOperand()
		{
			return GetChild(2);
		}

		private static void Check(IASTNode check, IASTNode first, IASTNode second)
		{
			var expectedTypeAwareNode = check as IExpectedTypeAwareNode;
			if (expectedTypeAwareNode != null)
			{
				IType expectedType = null;
				var firstNode = first as SqlNode;
				if (firstNode != null)
				{
					expectedType = firstNode.DataType;
				}
				if (expectedType == null)
				{
					var secondNode = second as SqlNode;
					if (secondNode != null)
					{
						expectedType = secondNode.DataType;
					}
				}
				expectedTypeAwareNode.ExpectedType = expectedType;
			}
		}
	}
}
