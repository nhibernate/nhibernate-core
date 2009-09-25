using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq
{
    public class QueryModelVisitor : QueryModelVisitorBase
	{
		private readonly HqlTreeBuilder _hqlTreeBuilder;
		private readonly ParameterAggregator _parameterAggregator;

        private readonly List<Action<IQuery>> _additionalCriteria = new List<Action<IQuery>>();
        private readonly List<LambdaExpression> _listTransformers = new List<LambdaExpression>();
        private readonly List<LambdaExpression> _itemTransformers = new List<LambdaExpression>();

        private readonly List<HqlTreeNode> _fromClauses = new List<HqlTreeNode>();
        private readonly List<HqlTreeNode> _whereClauses = new List<HqlTreeNode>();
        private HqlGroupBy _groupByClause;
        private HqlTreeNode _orderByClause;
		private HqlSelect _selectClause;

        private ResultOperatorProcessingMode _resultOperatorProcessingMode;

		private QueryModelVisitor(ParameterAggregator parameterAggregator)
		{
			_hqlTreeBuilder = new HqlTreeBuilder();
			_parameterAggregator = parameterAggregator;
		}

		public static CommandData GenerateHqlQuery(QueryModel queryModel)
		{
			return GenerateHqlQuery(queryModel, new ParameterAggregator());
		}

		public static CommandData GenerateHqlQuery(QueryModel queryModel, ParameterAggregator aggregator)
		{
            // Merge aggregating result operators (distinct, count, sum etc) into the select clause
            new MergeAggregatingResultsRewriter().ReWrite(queryModel);

            // Rewrite aggregate group-by statements
			new AggregatingGroupByRewriter().ReWrite(queryModel);

			// Swap out non-aggregating group-bys
			new NonAggregatingGroupByRewriter().ReWrite(queryModel);

            // Rewrite aggregating group-joins
		    new AggregatingGroupJoinRewriter().ReWrite(queryModel);

            // Flatten pointless subqueries
		    new QueryReferenceExpressionFlattener().ReWrite(queryModel);

			var visitor = new QueryModelVisitor(aggregator);
			visitor.VisitQueryModel(queryModel);
			return visitor.GetHqlCommand();
		}

		public CommandData GetHqlCommand()
		{
			HqlSelectFrom selectFrom = _hqlTreeBuilder.SelectFrom();

			if (_fromClauses.Count > 0)
			{
			    var from = _hqlTreeBuilder.From();

                foreach (var fromClause in _fromClauses)
                {
                    from.AddChild(fromClause);
                }

                selectFrom.AddChild(from);
			}

			if (_selectClause != null)
			{
				selectFrom.AddChild(_selectClause);
			}

			HqlQuery query = _hqlTreeBuilder.Query(selectFrom);

			if (_whereClauses.Count > 0)
			{
				query.AddChild(_hqlTreeBuilder.Where(MergeWhereClauses()));
			}

			if (_groupByClause != null)
			{
				query.AddChild(_groupByClause);
			}

			if (_orderByClause != null)
			{
				query.AddChild(_orderByClause);
			}

			return new CommandData(query,
			                       _parameterAggregator.GetParameters(),
                                   _itemTransformers,
			                       _listTransformers,
			                       _additionalCriteria);
		}

        private HqlTreeNode MergeWhereClauses()
        {
            HqlTreeNode output = _whereClauses[0];

            for (var i = 1; i < _whereClauses.Count; i++)
            {
                output = _hqlTreeBuilder.And(output, _whereClauses[i]);
            }

            return output;
        }

        public override void VisitQueryModel(QueryModel queryModel)
		{
		    _resultOperatorProcessingMode = ResultOperatorProcessingMode.ProcessServerSide;

			if (queryModel.MainFromClause != null)
			{
				queryModel.MainFromClause.Accept(this, queryModel);
			}

            VisitBodyClauses(queryModel.BodyClauses, queryModel);

            VisitResultOperators(queryModel.ResultOperators, queryModel);

			if (queryModel.SelectClause != null)
			{
				queryModel.SelectClause.Accept(this, queryModel);
			}

            // TODO - remove this "mode" enum and just add another list to a new derivation of QueryModel
            _resultOperatorProcessingMode = ResultOperatorProcessingMode.ProcessClientSide;

            VisitResultOperators(queryModel.ResultOperators, queryModel);
        }

		public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
		{
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameterAggregator);
			visitor.Visit(fromClause.FromExpression);

		    _fromClauses.Add(_hqlTreeBuilder.Range(
		                         visitor.GetHqlTreeNodes().Single(),
		                         _hqlTreeBuilder.Alias(fromClause.ItemName)));

			base.VisitMainFromClause(fromClause, queryModel);
		}


		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
            if (typeof(ClientSideTransformOperator).IsAssignableFrom(resultOperator.GetType()))
            {
                if (_resultOperatorProcessingMode == ResultOperatorProcessingMode.ProcessClientSide)
                {
                    ProcessClientSideResultOperator(resultOperator);
                }
            }
            else
            {
                if (_resultOperatorProcessingMode == ResultOperatorProcessingMode.ProcessServerSide)
                {
                    ProcessServerSideResultOperator(resultOperator);
                }
            }

			base.VisitResultOperator(resultOperator, queryModel, index);
		}

        private void ProcessServerSideResultOperator(ResultOperatorBase resultOperator)
        {
            if (resultOperator is FirstResultOperator)
            {
                ProcessFirstOperator();
            }
            else if (resultOperator is TakeResultOperator)
            {
                ProcessTakeOperator((TakeResultOperator)resultOperator);
            }
            else if (resultOperator is SkipResultOperator)
            {
                ProcessSkipOperator((SkipResultOperator)resultOperator);
            }
            else if (resultOperator is GroupResultOperator)
            {
                ProcessGroupByOperator((GroupResultOperator)resultOperator);
            }
            else
            {
                throw new NotSupportedException(string.Format("The {0} result operator is not current supported",
                                                              resultOperator.GetType().Name));
            }
        }

        private void ProcessClientSideResultOperator(ResultOperatorBase resultOperator)
        {
			if (resultOperator is NonAggregatingGroupBy)
			{
				ProcessNonAggregatingGroupBy((NonAggregatingGroupBy) resultOperator);
			}
            else if (resultOperator is ClientSideSelect)
            {
                ProcessClientSideSelect((ClientSideSelect) resultOperator);
            }
			else
			{
				throw new NotSupportedException(string.Format("The {0} result operator is not current supported",
				                                              resultOperator.GetType().Name));
			}
        }

        private void ProcessClientSideSelect(ClientSideSelect resultOperator)
        {
            var inputType = resultOperator.SelectClause.Parameters[0].Type;
            var outputType = resultOperator.SelectClause.Type.GetGenericArguments()[1];

            var inputList = Expression.Parameter(typeof (IEnumerable<>).MakeGenericType(inputType), "inputList");

            var selectMethod = EnumerableHelper.GetMethod("Select", new[] { typeof(IEnumerable<>), typeof(Func<,>) }, new[] { inputType, outputType });
            var toListMethod = EnumerableHelper.GetMethod("ToList", new[] {typeof (IEnumerable<>)}, new[] {outputType});

            var lambda = Expression.Lambda(
                            Expression.Call(toListMethod,
                                Expression.Call(selectMethod, inputList, resultOperator.SelectClause)), 
                            inputList);

            _listTransformers.Add(lambda);
        }

        private void ProcessTakeOperator(TakeResultOperator resultOperator)
		{
			_additionalCriteria.Add(q => q.SetMaxResults(resultOperator.GetConstantCount()));
		}

		private void ProcessSkipOperator(SkipResultOperator resultOperator)
		{
			_additionalCriteria.Add(q => q.SetFirstResult(resultOperator.GetConstantCount()));
		}

		private void ProcessNonAggregatingGroupBy(NonAggregatingGroupBy resultOperator)
		{
			// Stuff in the group by that doesn't map to HQL.  Run it client-side
			var listParameter = Expression.Parameter(typeof (IEnumerable<object>), "list");
			ParameterExpression itemParam = Expression.Parameter(resultOperator.GroupBy.ElementSelector.Type, "item");
			Expression keySelector = new GroupByKeySelectorVisitor(itemParam).Visit(resultOperator.GroupBy.KeySelector);

            var groupByMethod = EnumerableHelper.GetMethod("GroupBy", new[] { typeof(IEnumerable<>), typeof(Func<,>) }, new[] { resultOperator.GroupBy.ElementSelector.Type, resultOperator.GroupBy.KeySelector.Type });
            var castToObjectArray = EnumerableHelper.GetMethod("Cast", new[] { typeof(IEnumerable) }, new [] {typeof(object[])});
			var castToItem = EnumerableHelper.GetMethod("Cast", new [] {typeof(IEnumerable)}, new[] {resultOperator.GroupBy.ElementSelector.Type});
			var selectObject = EnumerableHelper.GetMethod("Select", new [] {typeof(IEnumerable<>), typeof(Func<,>)}, new[] { typeof (object[]), typeof (object)} );
			var toList = EnumerableHelper.GetMethod("ToList", new [] { typeof(IEnumerable<>)}, new [] {resultOperator.GroupBy.ItemType});

			LambdaExpression lambda = Expression.Lambda(keySelector, itemParam);

			ParameterExpression objectArrayParam = Expression.Parameter(typeof (object[]), "array");
			LambdaExpression index = Expression.Lambda(Expression.ArrayIndex(objectArrayParam, Expression.Constant(0)),
			                                           objectArrayParam);

		    _listTransformers.Add(Expression.Lambda(
		                                     Expression.Call(toList,
		                                                     Expression.Call(groupByMethod,
		                                                                     Expression.Call(castToItem,
		                                                                                     Expression.Call(selectObject,
		                                                                                                     Expression.Call(
		                                                                                                         castToObjectArray,
		                                                                                                         listParameter),
		                                                                                                     index)),
		                                                                     lambda)), listParameter));
		}

		private void ProcessGroupByOperator(GroupResultOperator resultOperator)
		{
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameterAggregator);
			visitor.Visit(resultOperator.KeySelector);
			_groupByClause = _hqlTreeBuilder.GroupBy();
			_groupByClause.AddChild(visitor.GetHqlTreeNodes().Single());
		}

		private void ProcessFirstOperator()
		{
			_additionalCriteria.Add(q => q.SetMaxResults(1));
		}

		private static bool CanBeEvaluatedInHqlSelectStatement(Expression expression)
		{
			return (expression.NodeType != ExpressionType.MemberInit) && (expression.NodeType != ExpressionType.New);
		}

		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
		{
            if (_selectClause != null)
            {
                return;
            }

            var visitor = new ProjectionEvaluator(_parameterAggregator, typeof(object[]), CanBeEvaluatedInHqlSelectStatement);

            visitor.Visit(selectClause.Selector);

            if (visitor.ProjectionExpression != null)
            {
                _itemTransformers.Add(visitor.ProjectionExpression);
            }

            _selectClause = _hqlTreeBuilder.Select(visitor.GetAstBuilderNode());

			base.VisitSelectClause(selectClause, queryModel);
		}

		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			// Visit the predicate to build the query
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameterAggregator);
			visitor.Visit(whereClause.Predicate);

			// There maybe a where clause in existence already, in which case we AND with it.
            _whereClauses.Add(visitor.GetHqlTreeNodes().Single());
		}

		public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
		{
			_orderByClause = _hqlTreeBuilder.OrderBy();

			foreach (Ordering clause in orderByClause.Orderings)
			{
				var visitor = new HqlGeneratorExpressionTreeVisitor(_parameterAggregator);
				visitor.Visit(clause.Expression);

				_orderByClause.AddChild(visitor.GetHqlTreeNodes().Single());
				_orderByClause.AddChild(clause.OrderingDirection == OrderingDirection.Asc
				                        	? _hqlTreeBuilder.Ascending()
				                        	: (HqlTreeNode) _hqlTreeBuilder.Descending());
			}

			base.VisitOrderByClause(orderByClause, queryModel, index);
		}

		public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
		{
            var fromVisitor = new HqlGeneratorExpressionTreeVisitor(_parameterAggregator);
            fromVisitor.Visit(joinClause.InnerSequence);

            var innerKey = new HqlGeneratorExpressionTreeVisitor(_parameterAggregator);
            innerKey.Visit(joinClause.InnerKeySelector);

            var outerKey = new HqlGeneratorExpressionTreeVisitor(_parameterAggregator);
            outerKey.Visit(joinClause.OuterKeySelector);

            _whereClauses.Add(_hqlTreeBuilder.Equality(innerKey.GetHqlTreeNodes().Single(), outerKey.GetHqlTreeNodes().Single()));

            _fromClauses.Add(_hqlTreeBuilder.Range(fromVisitor.GetHqlTreeNodes().Single(),
                                                   _hqlTreeBuilder.Alias(joinClause.ItemName)));
		}

		public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
		{
			if (fromClause.FromExpression is MemberExpression)
			{
				var member = (MemberExpression) fromClause.FromExpression;

				if (member.Expression is QuerySourceReferenceExpression)
				{
					// It's a join
					var visitor = new HqlGeneratorExpressionTreeVisitor(_parameterAggregator);
					visitor.Visit(fromClause.FromExpression);

				    _fromClauses.Add(_hqlTreeBuilder.Join(
				                         visitor.GetHqlTreeNodes().Single(),
				                         _hqlTreeBuilder.Alias(fromClause.ItemName)));
				}
			}

			base.VisitAdditionalFromClause(fromClause, queryModel, index);
		}

		public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
		{
            throw new NotImplementedException();
		}
	}

    public class QueryReferenceExpressionFlattener : NhExpressionTreeVisitor
    {
        public void ReWrite(QueryModel model)
        {
            model.TransformExpressions(VisitExpression);
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            var fromClauseBase = expression.ReferencedQuerySource as FromClauseBase;

            if (fromClauseBase != null && 
                fromClauseBase.FromExpression is QuerySourceReferenceExpression &&
                expression.Type == fromClauseBase.FromExpression.Type)
            {
                return fromClauseBase.FromExpression;
            }
            else
            {
                return base.VisitQuerySourceReferenceExpression(expression);
            }
        }
    }

    internal enum ResultOperatorProcessingMode
    {
        ProcessServerSide,
        ProcessClientSide
    }
}