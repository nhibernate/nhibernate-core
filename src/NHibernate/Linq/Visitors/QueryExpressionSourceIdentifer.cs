using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public class QueryExpressionSourceIdentifer : RelinqExpressionVisitor
	{
		private readonly QuerySourceIdentifier _identifier;

		public QueryExpressionSourceIdentifer(QuerySourceIdentifier identifier)
		{
			_identifier = identifier;
		}

		protected override Expression VisitSubQuery(SubQueryExpression expression)
		{
			_identifier.VisitQueryModel(expression.QueryModel);
			return base.VisitSubQuery(expression);
		}
	}
}
