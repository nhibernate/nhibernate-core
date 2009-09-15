using System;
using System.Linq.Expressions;
using Remotion.Data.Linq.Parsing;

namespace NHibernate.Linq
{
    public abstract class NhThrowingExpressionTreeVisitor : ThrowingExpressionTreeVisitor
    {
        protected override Expression VisitExpression(Expression expression)
        {
            switch ((NhExpressionType)expression.NodeType)
            {
                case NhExpressionType.Average:
                    return VisitNhAverage((AverageExpression)expression);
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
            return VisitUnhandledItem<DistinctExpression, Expression>(expression, "VisitNhDistinct", BaseVisitNhDistinct);
        }

        protected virtual Expression VisitNhAverage(AverageExpression expression)
        {
            return VisitUnhandledItem<AverageExpression, Expression>(expression, "VisitNhAverage", BaseVisitNhAverage);
        }

        protected virtual Expression VisitNhMin(MinExpression expression)
        {
            return VisitUnhandledItem<MinExpression, Expression>(expression, "VisitNhMin", BaseVisitNhMin);
        }

        protected virtual Expression VisitNhMax(MaxExpression expression)
        {
            return VisitUnhandledItem<MaxExpression, Expression>(expression, "VisitNhMax", BaseVisitNhMax);
        }

        protected virtual Expression VisitNhSum(SumExpression expression)
        {
            return VisitUnhandledItem<SumExpression, Expression>(expression, "VisitNhSum", BaseVisitNhSum);
        }

        protected virtual Expression VisitNhCount(CountExpression expression)
        {
            return VisitUnhandledItem<CountExpression, Expression>(expression, "VisitNhCount", BaseVisitNhCount);
        }

        protected virtual Expression BaseVisitNhCount(CountExpression expression)
        {
            return expression;
        }

        protected virtual Expression BaseVisitNhSum(SumExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new SumExpression(nx) : expression;
        }

        protected virtual Expression BaseVisitNhMax(MaxExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new MaxExpression(nx) : expression;
        }

        protected virtual Expression BaseVisitNhMin(MinExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new MinExpression(nx) : expression;
        }

        protected virtual Expression BaseVisitNhAverage(AverageExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new AverageExpression(nx) : expression;
        }

        private Expression BaseVisitNhDistinct(DistinctExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new DistinctExpression(nx) : expression;
        }

    }
}