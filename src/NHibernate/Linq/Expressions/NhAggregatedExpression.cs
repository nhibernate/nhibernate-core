using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class NhAggregatedExpression : Expression
	{
		public Expression Expression { get; set; }

		public NhAggregatedExpression(Expression expression, NhExpressionType type)
			: base((ExpressionType)type, expression.Type)
		{
			Expression = expression;
		}

		public NhAggregatedExpression(Expression expression, System.Type expressionType, NhExpressionType type)
			: base((ExpressionType)type, expressionType)
		{
			Expression = expression;
		}
	}
}