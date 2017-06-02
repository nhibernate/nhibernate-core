using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.GroupBy
{
	/// <summary>
	/// Detects if an expression tree contains naked QuerySourceReferenceExpression
	/// </summary>
	internal class IsNonAggregatingGroupByDetectionVisitor : NhExpressionVisitor
	{
		private bool _containsNakedQuerySourceReferenceExpression;

		public bool IsNonAggregatingGroupBy(Expression expression)
		{
			_containsNakedQuerySourceReferenceExpression = false;

			Visit(expression);

			return _containsNakedQuerySourceReferenceExpression;
		}

		protected override Expression VisitMember(MemberExpression expression)
		{
			return expression.IsGroupingKey()
				? expression
				: base.VisitMember(expression);
		}

		public override Expression VisitNhAggregate(NhAggregatedExpression expression)
			=> expression;

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
		{
			_containsNakedQuerySourceReferenceExpression = true;
			return expression;
		}
	}
}