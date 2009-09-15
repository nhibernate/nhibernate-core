using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Parsing;

namespace NHibernate.Linq
{
    // TODO: This needs strengthening.  For example, it doesn't recurse into SubQueries at present
    internal class AggregateDetectionVisitor : ExpressionTreeVisitor
    {
        public bool ContainsAggregateMethods { get; private set; }

        public bool Visit(Expression expression)
        {
            ContainsAggregateMethods = false;

            VisitExpression(expression);

            return ContainsAggregateMethods;
        }

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

            return base.VisitMethodCallExpression(m);
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            if (expression.QueryModel.ResultOperators.Count == 1
                && typeof(ValueFromSequenceResultOperatorBase).IsAssignableFrom(expression.QueryModel.ResultOperators[0].GetType()))
            {
                ContainsAggregateMethods = true;
            }

            return base.VisitSubQueryExpression(expression);
        }
    }
}