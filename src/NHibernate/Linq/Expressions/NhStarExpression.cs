using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Expressions
{
	public class NhStarExpression : NhExpression
	{
		public NhStarExpression(Expression expression)
		{
			Expression = expression;
		}

		public Expression Expression { get; }

		public override System.Type Type => Expression.Type;

		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			var newExpression = visitor.Visit(Expression);

			return newExpression != Expression
				? new NhStarExpression(newExpression)
				: this;
		}

		protected override Expression Accept(NhExpressionVisitor visitor)
		{
			return visitor.VisitNhStar(this);
		}
	}
}
