using System.Linq.Expressions;
using NHibernate.Linq.ReWriters;
using Remotion.Data.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
    public class NhExpressionTreeVisitor : ExpressionTreeVisitor
    {
        protected override Expression VisitExpression(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            switch ((NhExpressionType) expression.NodeType)
            {
                case NhExpressionType.Average:
                    return VisitNhAverage((NhAverageExpression) expression);
                case NhExpressionType.Min:
                    return VisitNhMin((NhMinExpression)expression);
                case NhExpressionType.Max:
                    return VisitNhMax((NhMaxExpression)expression);
                case NhExpressionType.Sum:
                    return VisitNhSum((NhSumExpression)expression);
                case NhExpressionType.Count:
                    return VisitNhCount((NhCountExpression)expression);
                case NhExpressionType.Distinct:
                    return VisitNhDistinct((NhDistinctExpression) expression);
                case NhExpressionType.New:
                    return VisitNhNew((NhNewExpression) expression);
            }

            return base.VisitExpression(expression);
        }

        private Expression VisitNhNew(NhNewExpression expression)
        {
            var arguments = VisitExpressionList(expression.Arguments);

            return arguments != expression.Arguments ? new NhNewExpression(expression.Members, arguments) : expression;
        }

        protected virtual Expression VisitNhDistinct(NhDistinctExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new NhDistinctExpression(nx) : expression;
        }

        protected virtual Expression VisitNhCount(NhCountExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new NhCountExpression(nx) : expression;
        }

        protected virtual Expression VisitNhSum(NhSumExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new NhSumExpression(nx) : expression;
        }

        protected virtual Expression VisitNhMax(NhMaxExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new NhMaxExpression(nx) : expression;
        }

        protected virtual Expression VisitNhMin(NhMinExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new NhMinExpression(nx) : expression;
        }

        protected virtual Expression VisitNhAverage(NhAverageExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new NhAverageExpression(nx) : expression;
        }
    }
}