using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Expressions
{
	public abstract class NhAggregatedExpression : ExtensionExpression
	{
		public Expression Expression { get; set; }

		protected NhAggregatedExpression(Expression expression, NhExpressionType type)
			: base(expression.Type, (ExpressionType)type)
		{
			Expression = expression;
		}

		protected NhAggregatedExpression(Expression expression, System.Type expressionType, NhExpressionType type)
			: base(expressionType, (ExpressionType)type)
		{
			Expression = expression;
		}

		protected override Expression VisitChildren(ExpressionTreeVisitor visitor)
		{
			var newExpression = visitor.VisitExpression(Expression);

			return newExpression != Expression
					   ? CreateNew(newExpression)
					   : this;
		}

		public abstract Expression CreateNew(Expression expression);
	}
}