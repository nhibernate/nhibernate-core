using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing;

namespace NHibernate.Linq
{
    internal class GroupBySelectorVisitor : ExpressionTreeVisitor
    {
        private readonly ParameterExpression _parameter;

        public GroupBySelectorVisitor(ParameterExpression parameter)
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