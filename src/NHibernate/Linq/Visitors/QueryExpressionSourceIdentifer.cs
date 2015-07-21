using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public class QueryExpressionSourceIdentifer : ExpressionTreeVisitor
	{
		private readonly QuerySourceIdentifier _identifier;

		public QueryExpressionSourceIdentifer(QuerySourceIdentifier identifier)
		{
			_identifier = identifier;
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			_identifier.VisitQueryModel(expression.QueryModel);
			return base.VisitSubQueryExpression(expression);
		}
	}
}