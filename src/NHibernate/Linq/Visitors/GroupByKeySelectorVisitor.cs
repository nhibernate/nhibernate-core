using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	internal class GroupByKeySourceFinder : ExpressionTreeVisitor
	{
		private Expression _source;

		public GroupByKeySourceFinder()
		{
		}

		public Expression Visit(Expression expression)
		{
			VisitExpression(expression);
			return _source;
		}

		protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			_source = expression;
			return expression;
		}
	}
}