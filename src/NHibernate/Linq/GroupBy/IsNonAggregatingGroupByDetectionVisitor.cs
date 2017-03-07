using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.GroupBy
{
	/// <summary>
	/// Detects if an expression tree contains naked QuerySourceReferenceExpression
	/// </summary>
	internal class IsNonAggregatingGroupByDetectionVisitor : NhExpressionTreeVisitor
	{
		private bool _containsNakedQuerySourceReferenceExpression;

		public bool IsNonAggregatingGroupBy(Expression expression)
		{
			_containsNakedQuerySourceReferenceExpression = false;

			VisitExpression(expression);

			return _containsNakedQuerySourceReferenceExpression;
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			return expression.IsGroupingKey()
					   ? expression
					   : base.VisitMemberExpression(expression);
		}

		protected override Expression VisitNhAggregate(NhAggregatedExpression expression)
		{
			return expression;
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			_containsNakedQuerySourceReferenceExpression = true;
			return expression;
		}
	}
}