using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.ReWriters;
using Remotion.Data.Linq.Clauses.Expressions;

namespace NHibernate.Linq.Visitors
{
    // TODO: This needs strengthening.  Possibly a lot in common with the GroupJoinAggregateDetectionVisitor class, which does many more checks
    internal class GroupByAggregateDetectionVisitor : NhExpressionTreeVisitor
    {
        public bool ContainsAggregateMethods { get; private set; }

        public bool Visit(Expression expression)
        {
            ContainsAggregateMethods = false;

            VisitExpression(expression);

            return ContainsAggregateMethods;
        }

        // TODO - this should not exist, since it should be handled either by re-linq or by the MergeAggregatingResultsRewriter
        protected override Expression VisitMethodCallExpression(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof (Queryable) ||
                m.Method.DeclaringType == typeof (Enumerable))
            {
                switch (m.Method.Name)
                {
                    case "Count":
                    case "Min":
                    case "Max":
                    case "Sum":
                    case "Average":
                        ContainsAggregateMethods = true;
                        break;
                }
            }

            return m;
        }

        // TODO - having a VisitNhAggregation method or something in the base class would remove this duplication...
        protected override Expression VisitNhAverage(NhAverageExpression expression)
        {
            ContainsAggregateMethods = true;
            return expression;
        }

        protected override Expression VisitNhCount(NhCountExpression expression)
        {
            ContainsAggregateMethods = true;
            return expression;
        }

        protected override Expression VisitNhMax(NhMaxExpression expression)
        {
            ContainsAggregateMethods = true;
            return expression;
        }

        protected override Expression VisitNhMin(NhMinExpression expression)
        {
            ContainsAggregateMethods = true;
            return expression;
        }

        protected override Expression VisitNhSum(NhSumExpression expression)
        {
            ContainsAggregateMethods = true;
            return expression;
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            ContainsAggregateMethods =
                new GroupByAggregateDetectionVisitor().Visit(expression.QueryModel.SelectClause.Selector);

            return expression;
        }
    }
}