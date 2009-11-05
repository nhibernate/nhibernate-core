using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;

namespace NHibernate.Linq.ReWriters
{
	public class QueryReferenceExpressionFlattener : NhExpressionTreeVisitor
	{
		private QueryReferenceExpressionFlattener()
		{
		}

		public static void ReWrite(QueryModel model)
		{
			var visitor = new QueryReferenceExpressionFlattener();
			model.TransformExpressions(visitor.VisitExpression);
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			var fromClauseBase = expression.ReferencedQuerySource as FromClauseBase;

			if (fromClauseBase != null && 
			    fromClauseBase.FromExpression is QuerySourceReferenceExpression &&
			    expression.Type == fromClauseBase.FromExpression.Type)
			{
				return fromClauseBase.FromExpression;
			}
			else
			{
				return base.VisitQuerySourceReferenceExpression(expression);
			}
		}
	}
}