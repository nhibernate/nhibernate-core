using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public class SwapQuerySourceVisitor : RelinqExpressionVisitor
	{
		private readonly IQuerySource _oldClause;
		private readonly IQuerySource _newClause;

		public SwapQuerySourceVisitor(IQuerySource oldClause, IQuerySource newClause)
		{
			_oldClause = oldClause;
			_newClause = newClause;
		}

		public Expression Swap(Expression expression)
		{
			return Visit(expression);
		}

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
		{
			if (expression.ReferencedQuerySource == _oldClause)
			{
				return new QuerySourceReferenceExpression(_newClause);
			}

			// TODO - really don't like this drill down approach.  Feels fragile
			var mainFromClause = expression.ReferencedQuerySource as MainFromClause;

			if (mainFromClause != null)
			{
				mainFromClause.FromExpression = Visit(mainFromClause.FromExpression);
			}

			return expression;
		}

		protected override Expression VisitSubQuery(SubQueryExpression expression)
		{
			expression.QueryModel.TransformExpressions(Visit);
			return base.VisitSubQuery(expression);
		}
	}
}