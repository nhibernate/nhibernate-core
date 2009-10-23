using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
    internal class GroupByKeySelectorVisitor : ExpressionTreeVisitor
    {
        private readonly ParameterExpression _parameter;

        public GroupByKeySelectorVisitor(ParameterExpression parameter)
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
}