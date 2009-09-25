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
                    return VisitNhAverage((NhAverageExpression)expression);
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

        protected virtual Expression VisitNhNew(NhNewExpression expression)
        {
            return VisitUnhandledItem<NhNewExpression, Expression>(expression, "VisitNhNew", BaseVisitNhNew);
        }

        protected virtual Expression VisitNhDistinct(NhDistinctExpression expression)
        {
            return VisitUnhandledItem<NhDistinctExpression, Expression>(expression, "VisitNhDistinct", BaseVisitNhDistinct);
        }

        protected virtual Expression VisitNhAverage(NhAverageExpression expression)
        {
            return VisitUnhandledItem<NhAverageExpression, Expression>(expression, "VisitNhAverage", BaseVisitNhAverage);
        }

        protected virtual Expression VisitNhMin(NhMinExpression expression)
        {
            return VisitUnhandledItem<NhMinExpression, Expression>(expression, "VisitNhMin", BaseVisitNhMin);
        }

        protected virtual Expression VisitNhMax(NhMaxExpression expression)
        {
            return VisitUnhandledItem<NhMaxExpression, Expression>(expression, "VisitNhMax", BaseVisitNhMax);
        }

        protected virtual Expression VisitNhSum(NhSumExpression expression)
        {
            return VisitUnhandledItem<NhSumExpression, Expression>(expression, "VisitNhSum", BaseVisitNhSum);
        }

        protected virtual Expression VisitNhCount(NhCountExpression expression)
        {
            return VisitUnhandledItem<NhCountExpression, Expression>(expression, "VisitNhCount", BaseVisitNhCount);
        }

        protected virtual Expression BaseVisitNhCount(NhCountExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new NhCountExpression(nx) : expression;
        }

        protected virtual Expression BaseVisitNhSum(NhSumExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new NhSumExpression(nx) : expression;
        }

        protected virtual Expression BaseVisitNhMax(NhMaxExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new NhMaxExpression(nx) : expression;
        }

        protected virtual Expression BaseVisitNhMin(NhMinExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new NhMinExpression(nx) : expression;
        }

        protected virtual Expression BaseVisitNhAverage(NhAverageExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new NhAverageExpression(nx) : expression;
        }

        protected Expression BaseVisitNhDistinct(NhDistinctExpression expression)
        {
            Expression nx = base.VisitExpression(expression.Expression);

            return nx != expression.Expression ? new NhDistinctExpression(nx) : expression;
        }

        protected Expression BaseVisitNhNew(NhNewExpression expression)
        {
            var arguments = base.VisitExpressionList(expression.Arguments);

            return arguments != expression.Arguments ? new NhNewExpression(expression.Members, arguments) : expression;
        }
    }
}