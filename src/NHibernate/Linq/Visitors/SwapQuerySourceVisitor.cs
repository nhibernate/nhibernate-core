using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public class SwapQuerySourceVisitor : ExpressionTreeVisitor
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
			return VisitExpression(expression);
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			if (expression.ReferencedQuerySource == _oldClause)
			{
				return new QuerySourceReferenceExpression(_newClause);
			}

			// TODO - really don't like this drill down approach.  Feels fragile
			var mainFromClause = expression.ReferencedQuerySource as MainFromClause;

			if (mainFromClause != null)
			{
				mainFromClause.FromExpression = VisitExpression(mainFromClause.FromExpression);
			}

			return expression;
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			expression.QueryModel.TransformExpressions(VisitExpression);
			return base.VisitSubQueryExpression(expression);
		}
	}
}