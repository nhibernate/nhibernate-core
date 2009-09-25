using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;

namespace NHibernate.Linq
{
    public class AggregatingGroupJoinRewriter
    {
        public void ReWrite(QueryModel model)
        {
            // We want to take queries like this:

            //var q =
            //    from c in db.Customers
            //    join o in db.Orders on c.CustomerId equals o.Customer.CustomerId into ords
            //    join e in db.Employees on c.Address.City equals e.Address.City into emps
            //    select new { c.ContactName, ords = ords.Count(), emps = emps.Count() };

            // and turn them into this:

            //var q =
            //    from c in db.Customers
            //    select new
            //    {
            //        c.ContactName,
            //        ords = (from o2 in db.Orders where o2.Customer.CustomerId == c.CustomerId select o2).Count(),
            //        emps = (from e2 in db.Employees where e2.Address.City == c.Address.City select e2).Count()
            //    };

            // so spot a group join where every use of the grouping in the selector is an aggregate

            // firstly, get the group join clauses
            var groupJoin = model.BodyClauses.Where(bc => bc is GroupJoinClause).Cast<GroupJoinClause>();

            if (groupJoin.Count() == 0)
            {
                // No group join here..
                return;
            }

            // Now walk the tree to decide which groupings are fully aggregated (and can hence be done in hql)
            var aggregateDetectorResults = IsAggregatingGroupJoin(model, groupJoin);

            if (aggregateDetectorResults.AggregatingClauses.Count > 0)
            {
                // Re-write the select expression
                model.SelectClause.TransformExpressions(s => GroupJoinSelectClauseRewriter.ReWrite(s, aggregateDetectorResults));

                // Remove the aggregating group joins
                foreach (GroupJoinClause aggregatingGroupJoin in aggregateDetectorResults.AggregatingClauses)
                {
                    model.BodyClauses.Remove(aggregatingGroupJoin);
                }
            }
        }

        private static IsAggregatingResults IsAggregatingGroupJoin(QueryModel model, IEnumerable<GroupJoinClause> clause)
        {
            return new GroupJoinAggregateDetectionVisitor(clause).Visit(model.SelectClause.Selector);
        }
    }

    public class GroupJoinSelectClauseRewriter : NhExpressionTreeVisitor
    {
        private readonly IsAggregatingResults _results;

        public static Expression ReWrite(Expression expression, IsAggregatingResults results)
        {
            return new GroupJoinSelectClauseRewriter(results).VisitExpression(expression);
        }

        private GroupJoinSelectClauseRewriter(IsAggregatingResults results)
        {
            _results = results;
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            // If the sub queries main (and only) from clause is one of our aggregating group bys, then swap it
            GroupJoinClause groupJoin = LocateGroupJoinQuerySource(expression.QueryModel);

            if (groupJoin != null)
            {
                Expression innerSelector = new SwapQuerySourceVisitor(groupJoin.JoinClause, expression.QueryModel.MainFromClause).
                    Swap(groupJoin.JoinClause.InnerKeySelector);

                expression.QueryModel.MainFromClause.FromExpression = groupJoin.JoinClause.InnerSequence;


                // TODO - this only works if the key selectors are not composite.  Needs improvement...
                expression.QueryModel.BodyClauses.Add(new WhereClause(Expression.Equal(innerSelector, groupJoin.JoinClause.OuterKeySelector)));
            }

            return expression;
        }

        private GroupJoinClause LocateGroupJoinQuerySource(QueryModel model)
        {
            if (model.BodyClauses.Count > 0)
            {
                return null;
            }
            return new LocateGroupJoinQuerySource(_results).Detect(model.MainFromClause.FromExpression);
        }
    }

    public class SwapQuerySourceVisitor : NhExpressionTreeVisitor
    {
        private readonly IQuerySource _oldClause;
        private readonly IQuerySource _newClause;

        public SwapQuerySourceVisitor(IQuerySource oldClause, IQuerySource newClause)
        {
            _oldClause = oldClause;
            _newClause = newClause;
        }

        public Expression Swap(Expression expression)
        {
            return VisitExpression(expression);
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            if (expression.ReferencedQuerySource == _oldClause)
            {
                return new QuerySourceReferenceExpression(_newClause);
            }

            // TODO - really don't like this drill down approach.  Feels fragile
            var mainFromClause = expression.ReferencedQuerySource as MainFromClause;

            if (mainFromClause != null)
            {
                mainFromClause.FromExpression = VisitExpression(mainFromClause.FromExpression);
            }

            return expression;
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            expression.QueryModel.TransformExpressions(VisitExpression);
            return base.VisitSubQueryExpression(expression);
        }
    }

    public class LocateGroupJoinQuerySource : NhExpressionTreeVisitor
    {
        private readonly IsAggregatingResults _results;
        private GroupJoinClause _groupJoin;

        public LocateGroupJoinQuerySource(IsAggregatingResults results)
        {
            _results = results;
        }

        public GroupJoinClause Detect(Expression expression)
        {
            VisitExpression(expression);
            return _groupJoin;    
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            if (_results.AggregatingClauses.Contains(expression.ReferencedQuerySource as GroupJoinClause))
            {
                _groupJoin = expression.ReferencedQuerySource as GroupJoinClause;
            }

            return base.VisitQuerySourceReferenceExpression(expression);
        }
    }

    public class IsAggregatingResults
    {
        public List<GroupJoinClause> NonAggregatingClauses  { get; set; }
        public List<GroupJoinClause> AggregatingClauses  { get; set; }
        public List<Expression> NonAggregatingExpressions { get; set; }
    }

    internal class GroupJoinAggregateDetectionVisitor : NhExpressionTreeVisitor
    {
        private readonly HashSet<GroupJoinClause> _groupJoinClauses;
        private readonly StackFlag _inAggregate = new StackFlag();
        private readonly StackFlag _parentExpressionProcessed = new StackFlag();

        private readonly List<Expression> _nonAggregatingExpressions = new List<Expression>();
        private readonly List<GroupJoinClause> _nonAggregatingGroupJoins = new List<GroupJoinClause>();
        private readonly List<GroupJoinClause> _aggregatingGroupJoins = new List<GroupJoinClause>();

        public GroupJoinAggregateDetectionVisitor(IEnumerable<GroupJoinClause> groupJoinClause)
        {
            _groupJoinClauses = new HashSet<GroupJoinClause>(groupJoinClause);
        }

        public IsAggregatingResults Visit(Expression expression)
        {
            VisitExpression(expression);

            return new IsAggregatingResults { NonAggregatingClauses = _nonAggregatingGroupJoins, AggregatingClauses = _aggregatingGroupJoins, NonAggregatingExpressions = _nonAggregatingExpressions };
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            VisitExpression(expression.QueryModel.SelectClause.Selector);
            return expression;
        }

        protected override Expression VisitNhAverage(NhAverageExpression expression)
        {
            using (_inAggregate.SetFlag())
            {
                return base.VisitNhAverage(expression);
            }
        }

        protected override Expression VisitNhCount(NhCountExpression expression)
        {
            using (_inAggregate.SetFlag())
            {
                return base.VisitNhCount(expression);
            }
        }

        protected override Expression VisitNhMax(NhMaxExpression expression)
        {
            using (_inAggregate.SetFlag())
            {
                return base.VisitNhMax(expression);
            }
        }

        protected override Expression VisitNhMin(NhMinExpression expression)
        {
            using (_inAggregate.SetFlag())
            {
                return base.VisitNhMin(expression);
            }
        }

        protected override Expression VisitNhSum(NhSumExpression expression)
        {
            using (_inAggregate.SetFlag())
            {
                return base.VisitNhSum(expression);
            }
        }

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            if (_inAggregate.FlagIsFalse && _parentExpressionProcessed.FlagIsFalse)
            {
                _nonAggregatingExpressions.Add(expression);
            }

            using (_parentExpressionProcessed.SetFlag())
            {
                return base.VisitMemberExpression(expression);
            }
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            var fromClause = (FromClauseBase) expression.ReferencedQuerySource;

            if (fromClause.FromExpression is QuerySourceReferenceExpression)
            {
                var querySourceReference = (QuerySourceReferenceExpression) fromClause.FromExpression;

                if (_groupJoinClauses.Contains(querySourceReference.ReferencedQuerySource as GroupJoinClause))
                {
                    if (_inAggregate.FlagIsFalse)
                    {
                        _nonAggregatingGroupJoins.Add((GroupJoinClause) querySourceReference.ReferencedQuerySource);
                    }
                    else
                    {
                        _aggregatingGroupJoins.Add((GroupJoinClause) querySourceReference.ReferencedQuerySource);
                    }
                }
            }

            return base.VisitQuerySourceReferenceExpression(expression);
        }

        internal class StackFlag
        {
            public bool FlagIsTrue { get; private set; }

            public bool FlagIsFalse { get { return !FlagIsTrue; } }

            public IDisposable SetFlag()
            {
                return new StackFlagDisposable(this);
            }

            internal class StackFlagDisposable : IDisposable
            {
                private readonly StackFlag _parent;
                private readonly bool _old;

                public StackFlagDisposable(StackFlag parent)
                {
                    _parent = parent;
                    _old = parent.FlagIsTrue;
                    parent.FlagIsTrue = true;
                }

                public void Dispose()
                {
                    _parent.FlagIsTrue = _old;
                }
            }
        }
    }
}