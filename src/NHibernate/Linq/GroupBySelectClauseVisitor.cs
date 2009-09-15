using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Parsing;

namespace NHibernate.Linq
{
    internal class GroupBySelectClauseVisitor : ExpressionTreeVisitor
    {
        public static Expression Visit(Expression expression)
        {
            var visitor = new GroupBySelectClauseVisitor();
            return visitor.VisitExpression(expression);
        }

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            if (expression.Member.Name == "Key" &&
                expression.Member.DeclaringType.GetGenericTypeDefinition() == typeof (IGrouping<,>))
            {
                var querySourceRef = expression.Expression as QuerySourceReferenceExpression;

                var fromClause = querySourceRef.ReferencedQuerySource as FromClauseBase;

                var subQuery = fromClause.FromExpression as SubQueryExpression;

                var groupBy =
                    subQuery.QueryModel.ResultOperators.Where(r => r is GroupResultOperator).Single() as
                    GroupResultOperator;

                return groupBy.KeySelector;
            }
            else
            {
                return base.VisitMemberExpression(expression);
            }
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            if (expression.QueryModel.ResultOperators.Count == 1)
            {
                ResultOperatorBase resultOperator = expression.QueryModel.ResultOperators[0];

                if (resultOperator is AverageResultOperator)
                {
                    return new AverageExpression(expression.QueryModel.SelectClause.Selector);
                }
                else if (resultOperator is MinResultOperator)
                {
                    return new MinExpression(expression.QueryModel.SelectClause.Selector);
                }
                else if (resultOperator is MaxResultOperator)
                {
                    return new MaxExpression(expression.QueryModel.SelectClause.Selector);
                }
                else if (resultOperator is CountResultOperator)
                {
                    return new CountExpression();
                }
                else if (resultOperator is SumResultOperator)
                {
                    return new SumExpression(expression.QueryModel.SelectClause.Selector);                    
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                return base.VisitSubQueryExpression(expression);
            }
        }
    }

    public enum NhExpressionType
    {
        Average = 10000,
        Min,
        Max,
        Sum,
        Count,
        Distinct
    }

    public class NhAggregatedExpression : Expression
    {
        public Expression Expression { get; set; }

        public NhAggregatedExpression(Expression expression, NhExpressionType type)
            : base((ExpressionType)type, expression.Type)
        {
            Expression = expression;
        }
    }

    public class AverageExpression : NhAggregatedExpression
    {
        public AverageExpression(Expression expression) : base(expression, NhExpressionType.Average)
        {
        }
    }

    public class MinExpression : NhAggregatedExpression
    {
        public MinExpression(Expression expression)
            : base(expression, NhExpressionType.Min)
        {
        }
    }

    public class MaxExpression : NhAggregatedExpression
    {
        public MaxExpression(Expression expression)
            : base(expression, NhExpressionType.Max)
        {
        }
    }

    public class SumExpression : NhAggregatedExpression
    {
        public SumExpression(Expression expression)
            : base(expression, NhExpressionType.Sum)
        {
        }
    }

    public class DistinctExpression : NhAggregatedExpression
    {
        public DistinctExpression(Expression expression)
            : base(expression, NhExpressionType.Distinct)
        {
        }
    }

    public class CountExpression : Expression
    {
        public CountExpression()
            : base((ExpressionType)NhExpressionType.Count, typeof(int))
        {
        }
    }
}
