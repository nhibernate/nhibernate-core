using Antlr.Runtime;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/**
	 * Contract for nodes representing logcial BETWEEN (ternary) operators.
	 *
	 * @author Steve Ebersole
	 */
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

		public IASTNode GetFixtureOperand()
		{
			return GetChild(0);
		}

		public IASTNode GetLowOperand()
		{
			return GetChild(1);
		}

		public IASTNode GetHighOperand()
		{
			return GetChild(2);
		}

		private static void Check(IASTNode check, IASTNode first, IASTNode second)
		{
			if (typeof(IExpectedTypeAwareNode).IsAssignableFrom(check.GetType()))
			{
				IType expectedType = null;
				if (typeof(SqlNode).IsAssignableFrom(first.GetType()))
				{
					expectedType = ((SqlNode)first).DataType;
				}
				if (expectedType == null && typeof(SqlNode).IsAssignableFrom(second.GetType()))
				{
					expectedType = ((SqlNode)second).DataType;
				}
				((IExpectedTypeAwareNode)check).ExpectedType = expectedType;
			}
		}
	}
}
