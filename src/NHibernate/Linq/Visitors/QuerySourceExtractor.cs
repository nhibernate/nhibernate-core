using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	internal class QuerySourceExtractor : RelinqExpressionVisitor
	{
		private IQuerySource _querySource;

		public static IQuerySource GetQuerySource(Expression expression)
		{
			var sourceExtractor = new QuerySourceExtractor();
			sourceExtractor.Visit(expression);
			return sourceExtractor._querySource;
		}

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
		{
			_querySource = expression.ReferencedQuerySource;
			return base.VisitQuerySourceReference(expression);
		}
	}
}
