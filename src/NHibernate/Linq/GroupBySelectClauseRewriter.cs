using System;
using System.Linq.Expressions;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq
{
    internal class GroupBySelectClauseRewriter : NhExpressionTreeVisitor
    {
        public static Expression ReWrite(Expression expression, GroupResultOperator groupBy, QueryModel model)
        {
            var visitor = new GroupBySelectClauseRewriter(groupBy, model);
            return visitor.VisitExpression(expression);
        }

        private readonly GroupResultOperator _groupBy;
        private readonly QueryModel _model;

        public GroupBySelectClauseRewriter(GroupResultOperator groupBy, QueryModel model)
        {
            _groupBy = groupBy;
            _model = model;
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            if (expression.ReferencedQuerySource == _groupBy)
            {
                return _groupBy.ElementSelector;
            }

            return base.VisitQuerySourceReferenceExpression(expression);
        }

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            if (IsMemberOfModel(expression))
            {
                if (expression.Member.Name == "Key")
                {
                    return _groupBy.KeySelector;
                }
                else
                {
                    Expression elementSelector = _groupBy.ElementSelector;

                    if ((elementSelector is MemberExpression) || (elementSelector is QuerySourceReferenceExpression))
                    {
                        // If ElementSelector is MemberExpression, just return
                        return base.VisitMemberExpression(expression);
                    }
                    else if (elementSelector is NewExpression)
                    {
                        // If ElementSelector is NewExpression, then search for member of name "get_" + originalMemberExpression.Member.Name
                        // TODO - this wouldn't handle nested initialisers.  Should do a tree walk to find the correct member
                        var nex = elementSelector as NewExpression;

                        int i = 0;
                        foreach (var member in nex.Members)
                        {
                            if (member.Name == "get_" + expression.Member.Name)
                            {
                                return nex.Arguments[i];
                            }
                            i++;
                        }

                        throw new NotImplementedException();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }
            else
            {
                return base.VisitMemberExpression(expression);
            }
        }

        // TODO - dislike this code intensly.  Should probably be a tree-walk in its own right
        private bool IsMemberOfModel(MemberExpression expression)
        {
            var querySourceRef = expression.Expression as QuerySourceReferenceExpression;

            if (querySourceRef == null)
            {
                return false;
            }

            var fromClause = querySourceRef.ReferencedQuerySource as FromClauseBase;

            if (fromClause == null)
            {
                return false;
            }

            var subQuery = fromClause.FromExpression as SubQueryExpression;

            if (subQuery != null)
            {
                return subQuery.QueryModel == _model;
            }

            var referencedQuery = fromClause.FromExpression as QuerySourceReferenceExpression;

            if (referencedQuery == null)
            {
                return false;
            }

            var querySource = referencedQuery.ReferencedQuerySource as FromClauseBase;

            var subQuery2 = querySource.FromExpression as SubQueryExpression;

            return (subQuery2.QueryModel == _model);
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            // TODO - is this safe?  All we are extracting is the select clause from the sub-query.  Assumes that everything
            // else in the subquery has been removed.  If there were two subqueries, one aggregating & one not, this may not be a 
            // valid assumption.  Should probably be passed a list of aggregating subqueries that we are flattening so that we can check...
            return GroupBySelectClauseRewriter.ReWrite(expression.QueryModel.SelectClause.Selector, _groupBy, _model);
        }
    }

    public enum NhExpressionType
    {
        Average = 10000,
        Min,
        Max,
        Sum,
        Count,
        Distinct,
        New
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

    public class NhAverageExpression : NhAggregatedExpression
    {
        public NhAverageExpression(Expression expression) : base(expression, NhExpressionType.Average)
        {
        }
    }

    public class NhMinExpression : NhAggregatedExpression
    {
        public NhMinExpression(Expression expression)
            : base(expression, NhExpressionType.Min)
        {
        }
    }

    public class NhMaxExpression : NhAggregatedExpression
    {
        public NhMaxExpression(Expression expression)
            : base(expression, NhExpressionType.Max)
        {
        }
    }

    public class NhSumExpression : NhAggregatedExpression
    {
        public NhSumExpression(Expression expression)
            : base(expression, NhExpressionType.Sum)
        {
        }
    }

    public class NhDistinctExpression : NhAggregatedExpression
    {
        public NhDistinctExpression(Expression expression)
            : base(expression, NhExpressionType.Distinct)
        {
        }
    }

    public class NhCountExpression : Expression
    {
        public NhCountExpression(Expression expression)
            : base((ExpressionType)NhExpressionType.Count, typeof(int))
        {
            Expression = expression;
        }

        public Expression Expression { get; private set; }
    }
}
