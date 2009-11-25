using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast;
using NHibernate.Linq.GroupBy;
using NHibernate.Linq.GroupJoin;
using NHibernate.Linq.ResultOperators;
using NHibernate.Linq.ReWriters;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors
{
    public class QueryModelVisitor : QueryModelVisitorBase
	{
		public static ExpressionToHqlTranslationResults GenerateHqlQuery(QueryModel queryModel, IDictionary<ConstantExpression, NamedParameter> parameters, IList<NamedParameterDescriptor> requiredHqlParameters)
		{
            // Remove unnecessary body operators
		    RemoveUnnecessaryBodyOperators.ReWrite(queryModel);

			// Merge aggregating result operators (distinct, count, sum etc) into the select clause
			MergeAggregatingResultsRewriter.ReWrite(queryModel);

			// Rewrite aggregate group-by statements
			AggregatingGroupByRewriter.ReWrite(queryModel);

			// Swap out non-aggregating group-bys
			NonAggregatingGroupByRewriter.ReWrite(queryModel);

			// Rewrite aggregating group-joins
			AggregatingGroupJoinRewriter.ReWrite(queryModel);

			// Rewrite non-aggregating group-joins
			NonAggregatingGroupJoinRewriter.ReWrite(queryModel);

			// Flatten pointless subqueries
			QueryReferenceExpressionFlattener.ReWrite(queryModel);

			var visitor = new QueryModelVisitor(parameters, requiredHqlParameters);
			visitor.VisitQueryModel(queryModel);

			return visitor.GetTranslation();
		}

		private readonly HqlTreeBuilder _hqlTreeBuilder;

        private readonly List<Action<IQuery, IDictionary<string, object>>> _additionalCriteria = new List<Action<IQuery, IDictionary<string, object>>>();
        private readonly List<LambdaExpression> _listTransformers = new List<LambdaExpression>();
        private readonly List<LambdaExpression> _itemTransformers = new List<LambdaExpression>();

        private readonly List<HqlTreeNode> _fromClauses = new List<HqlTreeNode>();
        private readonly List<HqlBooleanExpression> _whereClauses = new List<HqlBooleanExpression>();
        private HqlGroupBy _groupByClause;
        private HqlTreeNode _orderByClause;
		private HqlSelect _selectClause;

        private ResultOperatorProcessingMode _resultOperatorProcessingMode;
    	private readonly IDictionary<ConstantExpression, NamedParameter> _parameters;
    	private readonly IList<NamedParameterDescriptor> _requiredHqlParameters;

    	private QueryModelVisitor(IDictionary<ConstantExpression, NamedParameter> parameters, IList<NamedParameterDescriptor> requiredHqlParameters)
		{
			_parameters = parameters;
			_requiredHqlParameters = requiredHqlParameters;
			_hqlTreeBuilder = new HqlTreeBuilder();
		}

        public ExpressionToHqlTranslationResults GetTranslation()
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

            return new ExpressionToHqlTranslationResults(query,
                                                         _itemTransformers,
                                                         _listTransformers,
                                                         _additionalCriteria);
		}

        private HqlBooleanExpression MergeWhereClauses()
        {
            HqlBooleanExpression output = _whereClauses[0];

            for (var i = 1; i < _whereClauses.Count; i++)
            {
                output = _hqlTreeBuilder.BooleanAnd(output, _whereClauses[i]);
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
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);

		    _fromClauses.Add(_hqlTreeBuilder.Range(
                                 visitor.Visit(fromClause.FromExpression),
		                         _hqlTreeBuilder.Alias(fromClause.ItemName)));

			base.VisitMainFromClause(fromClause, queryModel);
		}


		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
            if (typeof(ClientSideTransformOperator).IsAssignableFrom(resultOperator.GetType()))
            {
                if (_resultOperatorProcessingMode == ResultOperatorProcessingMode.ProcessClientSide)
                {
                    ProcessClientSideResultOperator(resultOperator, queryModel);
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

        private void ProcessClientSideResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel)
        {
			if (resultOperator is NonAggregatingGroupBy)
			{
				ProcessNonAggregatingGroupBy((NonAggregatingGroupBy) resultOperator, queryModel);
			}
            else if (resultOperator is ClientSideSelect)
            {
                ProcessClientSideSelect((ClientSideSelect) resultOperator);
            }
            else if (resultOperator is AggregateResultOperator)
            {
                ProcessAggregateOperator((AggregateResultOperator)resultOperator);
            }
            else
			{
				throw new NotSupportedException(string.Format("The {0} result operator is not current supported",
				                                              resultOperator.GetType().Name));
			}
        }

        private void ProcessAggregateOperator(AggregateResultOperator resultOperator)
        {
            var inputType = resultOperator.Accumulator.Parameters[1].Type;
            var accumulatorType = resultOperator.Accumulator.Parameters[0].Type;
            var inputList = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(typeof(object)), "inputList");

            var castToItem = EnumerableHelper.GetMethod("Cast", new[] { typeof(IEnumerable) }, new[] { inputType });
            var castToItemExpr = Expression.Call(castToItem, inputList);

            MethodCallExpression call;

            if (resultOperator.ParseInfo.ParsedExpression.Arguments.Count == 2)
            {
                var aggregate = ReflectionHelper.GetMethod(() => Enumerable.Aggregate<object>(null, null));
                aggregate = aggregate.GetGenericMethodDefinition().MakeGenericMethod(inputType);

                call = Expression.Call(
                    aggregate,
                    castToItemExpr,
                    resultOperator.Accumulator
                    );
                
            }
            else if (resultOperator.ParseInfo.ParsedExpression.Arguments.Count == 3)
            {
                var aggregate = ReflectionHelper.GetMethod(() => Enumerable.Aggregate<object, object>(null, null, null));
                aggregate = aggregate.GetGenericMethodDefinition().MakeGenericMethod(inputType, accumulatorType);

                call = Expression.Call(
                    aggregate,
                    castToItemExpr,
                    resultOperator.OptionalSeed,
                    resultOperator.Accumulator
                    );
            }
            else
            {
                var selectorType = resultOperator.OptionalSelector.Type.GetGenericArguments()[2];
                var aggregate = ReflectionHelper.GetMethod(() => Enumerable.Aggregate<object, object, object>(null, null, null, null));
                aggregate = aggregate.GetGenericMethodDefinition().MakeGenericMethod(inputType, accumulatorType, selectorType);

                call = Expression.Call(
                    aggregate,
                    castToItemExpr,
                    resultOperator.OptionalSeed,
                    resultOperator.Accumulator,
                    resultOperator.OptionalSelector
                    );
            }

            _listTransformers.Add(Expression.Lambda(call, inputList));
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
            NamedParameter parameterName;

            // TODO - very similar to ProcessSkip, plus want to investigate the scenario in the "else"
            // clause to see if it is valid
            if (_parameters.TryGetValue(resultOperator.Count as ConstantExpression, out parameterName))
            {
                _additionalCriteria.Add((q, p) => q.SetMaxResults((int) p[parameterName.Name]));
            }
            else
            {
                _additionalCriteria.Add((q, p) => q.SetMaxResults(resultOperator.GetConstantCount()));
            }
		}

		private void ProcessSkipOperator(SkipResultOperator resultOperator)
		{
            NamedParameter parameterName;

            if (_parameters.TryGetValue(resultOperator.Count as ConstantExpression, out parameterName))
            {
                _additionalCriteria.Add((q, p) => q.SetFirstResult((int)p[parameterName.Name]));
            }
            else
            {
                _additionalCriteria.Add((q, p) => q.SetFirstResult(resultOperator.GetConstantCount()));
            }
        }

		private void ProcessNonAggregatingGroupBy(NonAggregatingGroupBy resultOperator, QueryModel model)
		{
			var tSource = model.SelectClause.Selector.Type;
			var tKey = resultOperator.GroupBy.KeySelector.Type;
			var tElement = resultOperator.GroupBy.ElementSelector.Type;

			// Stuff in the group by that doesn't map to HQL.  Run it client-side
			var listParameter = Expression.Parameter(typeof (IEnumerable<object>), "list");

			ParameterExpression itemParam = Expression.Parameter(tSource, "item");
			Expression keySelectorSource = itemParam;

			if (tSource != SourceOf(resultOperator.GroupBy.KeySelector))
			{
				keySelectorSource = Expression.MakeMemberAccess(itemParam,
																 tSource.GetMember(
																	((QuerySourceReferenceExpression)
																	 resultOperator.GroupBy.KeySelector).ReferencedQuerySource.
																		ItemName)[0]);
			}


			Expression keySelector = new GroupByKeySelectorVisitor(keySelectorSource).Visit(resultOperator.GroupBy.KeySelector);

			Expression elementSelectorSource = itemParam;

			if (tSource != SourceOf(resultOperator.GroupBy.ElementSelector))
			{
				elementSelectorSource = Expression.MakeMemberAccess(itemParam,
																 tSource.GetMember(
																	((QuerySourceReferenceExpression)
																	 resultOperator.GroupBy.ElementSelector).ReferencedQuerySource.
																		ItemName)[0]);
			}

			Expression elementSelector = new GroupByKeySelectorVisitor(elementSelectorSource).Visit(resultOperator.GroupBy.ElementSelector);

			var groupByMethod = EnumerableHelper.GetMethod("GroupBy",
				new[] { typeof(IEnumerable<>), typeof(Func<,>), typeof(Func<,>) }, 
				new[] { tSource, tKey, tElement });

			var castToItem = EnumerableHelper.GetMethod("Cast", new[] { typeof(IEnumerable) }, new[] { tSource });
			
			var toList = EnumerableHelper.GetMethod("ToList", new [] { typeof(IEnumerable<>)}, new [] {resultOperator.GroupBy.ItemType});

			LambdaExpression keySelectorExpr = Expression.Lambda(keySelector, itemParam);

			LambdaExpression elementSelectorExpr = Expression.Lambda(elementSelector, itemParam);

			Expression castToItemExpr = Expression.Call(castToItem, listParameter);

			var groupByExpr = Expression.Call(groupByMethod, castToItemExpr, keySelectorExpr, elementSelectorExpr);

			var toListExpr = Expression.Call(toList, groupByExpr);

			var lambdaExpr = Expression.Lambda(toListExpr, listParameter);

			_listTransformers.Add(lambdaExpr);
			
			return;
			/*
		    _listTransformers.Add(Expression.Lambda(
		                                     Expression.Call(toList,
		                                                     Expression.Call(groupByMethod,
		                                                                     Expression.Call(castToItem,
		                                                                                     Expression.Call(selectObject,
		                                                                                                     Expression.Call(
		                                                                                                         castToObjectArray,
		                                                                                                         listParameter),
		                                                                                                     index)),
		                                                                     keySelectorExpr)
															), 
														listParameter));*/
		}

    	private static System.Type SourceOf(Expression keySelector)
    	{
    		return new GroupByKeySourceFinder().Visit(keySelector).Type;
    	}

    	private void ProcessGroupByOperator(GroupResultOperator resultOperator)
		{
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);

            _groupByClause = _hqlTreeBuilder.GroupBy(visitor.Visit(resultOperator.KeySelector).AsExpression());
		}

		private void ProcessFirstOperator()
		{
			_additionalCriteria.Add((q, p) => q.SetMaxResults(1));
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

            var visitor = new ProjectionEvaluator(typeof(object[]), CanBeEvaluatedInHqlSelectStatement, _parameters, _requiredHqlParameters);

            visitor.Visit(selectClause.Selector);

            if (visitor.ProjectionExpression != null)
            {
                _itemTransformers.Add(visitor.ProjectionExpression);
            }

            _selectClause = _hqlTreeBuilder.Select(visitor.GetHqlNodes());

			base.VisitSelectClause(selectClause, queryModel);
		}

		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			// Visit the predicate to build the query
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);			

            _whereClauses.Add(visitor.Visit(whereClause.Predicate).AsBooleanExpression());
		}

		public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
		{
			_orderByClause = _hqlTreeBuilder.OrderBy();

			foreach (Ordering clause in orderByClause.Orderings)
			{
				var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);

                _orderByClause.AddChild(visitor.Visit(clause.Expression));
				_orderByClause.AddChild(clause.OrderingDirection == OrderingDirection.Asc
				                        	? _hqlTreeBuilder.Ascending()
				                        	: (HqlTreeNode) _hqlTreeBuilder.Descending());
			}
		}

		public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
		{
			var equalityVisitor = new EqualityHqlGenerator(_parameters, _requiredHqlParameters);
			var whereClause = equalityVisitor.Visit(joinClause.InnerKeySelector, joinClause.OuterKeySelector);

			var fromVisitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);

            _whereClauses.Add(whereClause);

            _fromClauses.Add(_hqlTreeBuilder.Range(fromVisitor.Visit(joinClause.InnerSequence),
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
					var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);

				    _fromClauses.Add(_hqlTreeBuilder.Join(
                                         visitor.Visit(fromClause.FromExpression).AsExpression(),
				                         _hqlTreeBuilder.Alias(fromClause.ItemName)));
				}
				else
				{
					// What's this?
					throw new NotSupportedException();
				}
			}
			else
			{
				// TODO - exact same code as in MainFromClause; refactor this out
				var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);

				_fromClauses.Add(_hqlTreeBuilder.Range(
                                     visitor.Visit(fromClause.FromExpression),
									 _hqlTreeBuilder.Alias(fromClause.ItemName)));

			}

			base.VisitAdditionalFromClause(fromClause, queryModel, index);
		}

		public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
		{
            throw new NotImplementedException();
		}

		internal enum ResultOperatorProcessingMode
		{
			ProcessServerSide,
			ProcessClientSide
		}
	}
}