using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
    internal class GroupByKeySelectorVisitor : ExpressionTreeVisitor
    {
        private readonly Expression _parameter;

        public GroupByKeySelectorVisitor(Expression parameter)
        {
            _parameter = parameter;
        }

        public Expression Visit(Expression expression)
        {
            return VisitExpression(expression);
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            return _parameter;
        }
    }

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