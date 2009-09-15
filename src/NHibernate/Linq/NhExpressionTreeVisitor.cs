using System.Linq.Expressions;
using Remotion.Data.Linq.Parsing;

namespace NHibernate.Linq
{
    public class NhExpressionTreeVisitor : ExpressionTreeVisitor
    {
        protected override Expression VisitExpression(Expression expression)
        {
            switch ((NhExpressionType) expression.NodeType)
            {
                case NhExpressionType.Average:
                    return VisitNhAverage((AverageExpression) expression);
                case NhExpressionType.Min:
                    return VisitNhMin((MinExpression)expression);
                case NhExpressionType.Max:
                    return VisitNhMax((MaxExpression)expression);
                case NhExpressionType.Sum:
                    return VisitNhSum((SumExpression)expression);
                case NhExpressionType.Count:
                    return VisitNhCount((CountExpression)expression);
                case NhExpressionType.Distinct:
                    return VisitNhDistinct((DistinctExpression) expression);
            }

            return base.VisitExpression(expression);
        }

        protected virtual Expression VisitNhDistinct(DistinctExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new DistinctExpression(nx) : expression;
        }

        protected virtual Expression VisitNhCount(CountExpression expression)
        {
            return expression;
        }

        protected virtual Expression VisitNhSum(SumExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new SumExpression(nx) : expression;
        }

        protected virtual Expression VisitNhMax(MaxExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new MaxExpression(nx) : expression;
        }

        protected virtual Expression VisitNhMin(MinExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new MinExpression(nx) : expression;
        }

        protected virtual Expression VisitNhAverage(AverageExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new AverageExpression(nx) : expression;
        }
    }
}