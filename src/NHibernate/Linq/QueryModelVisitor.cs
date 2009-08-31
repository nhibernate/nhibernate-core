using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Clauses.StreamedData;
using Remotion.Data.Linq.Transformations;

namespace NHibernate.Linq
{
    public class QueryModelVisitor : QueryModelVisitorBase
    {
        public static CommandData GenerateHqlQuery(QueryModel queryModel)
        {
            return GenerateHqlQuery(queryModel, new ParameterAggregator());
        }

        public static CommandData GenerateHqlQuery(QueryModel queryModel, ParameterAggregator aggregator)
        {
           // SubQueryFromClauseFlattener flattener = new SubQueryFromClauseFlattener();
           // flattener.VisitQueryModel(queryModel);

            var visitor = new QueryModelVisitor(aggregator);
            visitor.VisitQueryModel(queryModel);
            return visitor.GetHqlCommand();
        }

        private readonly HqlTreeBuilder _hqlTreeBuilder;
        private readonly ParameterAggregator _parameterAggregator;
        private readonly ParameterExpression _objectArray;
        private Expression _projectionExpression;
        private IStreamedDataInfo _inputInfo;
        readonly List<Action<IQuery>> _additionalCriteria = new List<Action<IQuery>>();


        private HqlWhere _whereClause;
        private HqlSelect _selectClause;
        private HqlFrom _fromClause;
        private readonly List<HqlTreeNode> _orderByClauses = new List<HqlTreeNode>();

        private QueryModelVisitor(ParameterAggregator parameterAggregator)
        {
            _hqlTreeBuilder = new HqlTreeBuilder();
            _parameterAggregator = parameterAggregator;
            _objectArray = Expression.Parameter(typeof (object[]), "objectArray");
        }

        public CommandData GetHqlCommand()
        {
            HqlSelectFrom selectFrom = _hqlTreeBuilder.SelectFrom();

            if (_fromClause != null)
            {
                selectFrom.AddChild(_fromClause);
            }

            if (_selectClause != null)
            {
                selectFrom.AddChild(_selectClause);
            }

            HqlQuery query = _hqlTreeBuilder.Query(selectFrom);

            if (_whereClause != null)
            {
                query.AddChild(_whereClause);
            }

            foreach (var orderByClause in _orderByClauses)
            {
                query.AddChild(orderByClause);
            }

            return new CommandData(query, 
                                   _parameterAggregator.GetParameters(), 
                                   _projectionExpression == null ? null : Expression.Lambda(_projectionExpression, _objectArray),
                                   _additionalCriteria);
        }

        public override void VisitQueryModel(QueryModel queryModel)
        {
            if (queryModel.MainFromClause != null)
            {
                queryModel.MainFromClause.Accept(this, queryModel);
            }

            if (queryModel.SelectClause != null)
            {
                queryModel.SelectClause.Accept(this, queryModel);
            }

            VisitBodyClauses(queryModel.BodyClauses, queryModel);

            VisitResultOperators(queryModel.ResultOperators, queryModel);
        }
        
        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            var visitor = new NhExpressionTreeVisitor(_parameterAggregator);
            visitor.Visit(fromClause.FromExpression);

            _fromClause = _hqlTreeBuilder.From(
                            _hqlTreeBuilder.Range(
                                visitor.GetAstBuilderNode().Single(),
                                _hqlTreeBuilder.Alias(fromClause.ItemName)));
            
            base.VisitMainFromClause(fromClause, queryModel);
        }

        
        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            if (IsCountOperator(resultOperator))
            {
                ProcessCountOperator(resultOperator);
            }
            else if (IsAggregateOperator(resultOperator))
            {
                ProcessAggregateOperator(resultOperator);
            }
            else if (IsPositionalOperator(resultOperator))
            {
                ProcessPositionalOperator(resultOperator);
            }
            else
            {
                throw new NotSupportedException(string.Format("The {0} result operator is not current supported",
                                                              resultOperator.GetType().Name));
            }

            base.VisitResultOperator(resultOperator, queryModel, index);
        }

        private void ProcessPositionalOperator(ResultOperatorBase resultOperator)
        {
            var first = (FirstResultOperator) resultOperator;

            _additionalCriteria.Add(q => q.SetMaxResults(1));
        }

        private bool IsPositionalOperator(ResultOperatorBase resultOperator)
        {
            return resultOperator is FirstResultOperator;
        }

        private void ProcessAggregateOperator(ResultOperatorBase resultOperator)
        {
            HqlTreeNode aggregateNode;

            if (resultOperator is AverageResultOperator)
            {
                aggregateNode = _hqlTreeBuilder.Average();
            }
            else if (resultOperator is SumResultOperator)
            {
                aggregateNode = _hqlTreeBuilder.Sum();
            }
            else if (resultOperator is MinResultOperator)
            {
                aggregateNode = _hqlTreeBuilder.Min();
            }
            else
            {
                aggregateNode = _hqlTreeBuilder.Max();
            }

            _inputInfo = resultOperator.GetOutputDataInfo(_inputInfo);

            HqlTreeNode child = _selectClause.Children.Single();

            _selectClause.ClearChildren();

            aggregateNode.AddChild(child);

            _selectClause.AddChild(_hqlTreeBuilder.Cast(aggregateNode, _inputInfo.DataType));
        }

        private void ProcessCountOperator(ResultOperatorBase resultOperator)
        {
            HqlTreeNode count = _hqlTreeBuilder.Count(_hqlTreeBuilder.RowStar());

            if (resultOperator is CountResultOperator)
            {
                // Need to cast to an int (Hql defaults to long counts)
                count = _hqlTreeBuilder.Cast(count, typeof (int));
            }

            _selectClause.ClearChildren();
            _selectClause.AddChild(count);
        }

        private static bool IsAggregateOperator(ResultOperatorBase resultOperator)
        {
            return resultOperator is MinResultOperator
                   || resultOperator is MaxResultOperator
                   || resultOperator is SumResultOperator
                   || resultOperator is AverageResultOperator;
        }

        private static bool IsCountOperator(ResultOperatorBase resultOperator)
        {
            return resultOperator is CountResultOperator || resultOperator is LongCountResultOperator;
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            var visitor = new ProjectionEvaluator(_parameterAggregator, _objectArray);
            visitor.Visit(selectClause.Selector);

            _projectionExpression = visitor.ProjectionExpression;

            _selectClause = _hqlTreeBuilder.Select(visitor.GetAstBuilderNode());

            _inputInfo = selectClause.GetOutputDataInfo();

            base.VisitSelectClause(selectClause, queryModel);
        }
        

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            // Visit the predicate to build the query
            var visitor = new NhExpressionTreeVisitor(_parameterAggregator);
            visitor.Visit(whereClause.Predicate);

            // There maybe a where clause in existence already, in which case we AND with it.
            if (_whereClause == null)
            {
                _whereClause = _hqlTreeBuilder.Where(visitor.GetAstBuilderNode().Single());
            }
            else
            {
                HqlAnd mergedPredicates = _hqlTreeBuilder.And(_whereClause.Children.Single(), visitor.GetAstBuilderNode().Single());
                _whereClause = _hqlTreeBuilder.Where(mergedPredicates);
            }
        }

        public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
        {
            var orderBys = new List<HqlOrderBy>();

            foreach (var clause in orderByClause.Orderings)
            {
                var visitor = new NhExpressionTreeVisitor(_parameterAggregator);
                visitor.Visit(clause.Expression);

                orderBys.Add(
                    _hqlTreeBuilder.OrderBy(visitor.GetAstBuilderNode().Single(),
                                        clause.OrderingDirection == OrderingDirection.Asc ? HqlDirection.Ascending : HqlDirection.Descending));
            }

            if (orderBys.Count > 1)
            {
                throw new NotImplementedException();
            }

            _orderByClauses.Add(orderBys[0]);

            base.VisitOrderByClause(orderByClause, queryModel, index);
        }

        /*
        public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
        {
            // HQL joins work differently, need to simulate using a cross join with a where condition

            _queryParts.AddFromPart(joinClause);
            _queryParts.AddWherePart(
                "({0} = {1})",
                GetHqlExpression(joinClause.OuterKeySelector),
                GetHqlExpression(joinClause.InnerKeySelector));

            base.VisitJoinClause(joinClause, queryModel, index);
        }
        */
        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            if (fromClause.FromExpression is MemberExpression)
            {
                var member = (MemberExpression)fromClause.FromExpression;

                if (member.Expression is QuerySourceReferenceExpression)
                {
                    // It's a join
                    var visitor = new NhExpressionTreeVisitor(_parameterAggregator);
                    visitor.Visit(fromClause.FromExpression);

                    var joinNode = _hqlTreeBuilder.Join(
                                        visitor.GetAstBuilderNode().Single(),
                                        _hqlTreeBuilder.Alias(fromClause.ItemName));

                    _fromClause.AddChild(joinNode);
                }   
            }

            base.VisitAdditionalFromClause(fromClause, queryModel, index);
        }
        /*
        public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
        {
            throw new NotSupportedException("Adding a join ... into ... implementation to the query provider is left to the reader for extra points.");
        }
        */
    }
}