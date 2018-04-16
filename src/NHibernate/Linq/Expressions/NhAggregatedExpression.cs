using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Expressions
{
	public abstract class NhAggregatedExpression : NhExpression
	{
		protected NhAggregatedExpression(Expression expression)
			: this(expression, expression.Type)
		{
		}

		protected NhAggregatedExpression(Expression expression, System.Type type)
		{
			Expression = expression;
			Type = type;
		}

		public sealed override System.Type Type { get; }

		public Expression Expression { get; }

		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			var newExpression = visitor.Visit(Expression);

			return newExpression != Expression
				? CreateNew(newExpression)
				: this;
		}

		public abstract Expression CreateNew(Expression expression);

		protected override Expression Accept(NhExpressionVisitor visitor)
		{
			return visitor.VisitNhAggregated(this);
		}
	}
}
