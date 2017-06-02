using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Expressions
{
	public abstract class NhAggregatedExpression : NhSimpleExpression
	{
		protected NhAggregatedExpression(Expression expression)
			: base(expression) { }

		protected NhAggregatedExpression(Expression expression, System.Type expressionType)
			: base(expression, expressionType) { }

		protected override Expression Accept(NhExpressionVisitor visitor)
			=> visitor.VisitNhAggregate(this);
	}
}