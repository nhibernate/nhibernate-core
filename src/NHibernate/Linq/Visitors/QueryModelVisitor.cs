using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast;
using NHibernate.Linq.GroupBy;
using NHibernate.Linq.GroupJoin;
using NHibernate.Linq.ResultOperators;
using NHibernate.Linq.ReWriters;
using NHibernate.Type;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Clauses.StreamedData;

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

        private readonly List<Action<IQuery, IDictionary<string, Tuple<object, IType>>>> _additionalCriteria = new List<Action<IQuery, IDictionary<string, Tuple<object, IType>>>>();
        private readonly List<LambdaExpression> _listTransformers = new List<LambdaExpression>();
        private readonly List<LambdaExpression> _itemTransformers = new List<LambdaExpression>();
        private readonly List<LambdaExpression> _postExecuteTransformers = new List<LambdaExpression>();

        private IStreamedDataInfo _previousEvaluationType;
        private IStreamedDataInfo _currentEvaluationType;

    	private readonly IDictionary<ConstantExpression, NamedParameter> _parameters;
    	private readonly IList<NamedParameterDescriptor> _requiredHqlParameters;
        private bool _serverSide = true;

        private HqlTreeNode _treeNode;

        private QueryModelVisitor(IDictionary<ConstantExpression, NamedParameter> parameters, IList<NamedParameterDescriptor> requiredHqlParameters)
		{
			_parameters = parameters;
			_requiredHqlParameters = requiredHqlParameters;
			_hqlTreeBuilder = new HqlTreeBuilder();
            _treeNode = _hqlTreeBuilder.Query(_hqlTreeBuilder.SelectFrom(_hqlTreeBuilder.From()));
		}

        public ExpressionToHqlTranslationResults GetTranslation()
		{
            return new ExpressionToHqlTranslationResults(_treeNode,
                                                         _itemTransformers,
                                                         _listTransformers,
                                                         _postExecuteTransformers,
                                                         _additionalCriteria);
		}

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
		{
            AddFromClause(_hqlTreeBuilder.Range(
                             HqlGeneratorExpressionTreeVisitor.Visit(fromClause.FromExpression, _parameters, _requiredHqlParameters),
                             _hqlTreeBuilder.Alias(fromClause.ItemName)));

			base.VisitMainFromClause(fromClause, queryModel);
		}


        private void AddWhereClause(HqlBooleanExpression where)
        {
            var currentWhere = _treeNode.NodesPreOrder.Where(n => n is HqlWhere).FirstOrDefault();

            if (currentWhere == null)
            {
                currentWhere = _hqlTreeBuilder.Where(where);
                _treeNode.As<HqlQuery>().AddChild(currentWhere);
            }
            else
            {
                var currentClause = (HqlBooleanExpression)currentWhere.Children.Single();

                currentWhere.ClearChildren();
                currentWhere.AddChild(_hqlTreeBuilder.BooleanAnd(currentClause, where));
            }
        }

        private void AddFromClause(HqlTreeNode from)
        {
            _treeNode.NodesPreOrder.Where(n => n is HqlFrom).First().AddChild(from);
        }

        private void AddSelectClause(HqlTreeNode select)
        {
            _treeNode.NodesPreOrder.Where(n => n is HqlSelectFrom).First().AddChild(select);
        }

        private void AddGroupByClause(HqlGroupBy groupBy)
        {
            _treeNode.As<HqlQuery>().AddChild(groupBy);
        }

        private void AddOrderByClause(HqlExpression orderBy, HqlDirectionStatement direction)
        {
            var orderByRoot = _treeNode.NodesPreOrder.Where(n => n is HqlOrderBy).FirstOrDefault();

            if (orderByRoot == null)
            {
                orderByRoot = _hqlTreeBuilder.OrderBy();
                _treeNode.As<HqlQuery>().AddChild(orderByRoot);
            }
            
            orderByRoot.AddChild(orderBy);
            orderByRoot.AddChild(direction);
        }


        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
		    _previousEvaluationType = _currentEvaluationType;
            _currentEvaluationType = resultOperator.GetOutputDataInfo(_previousEvaluationType);

            if (resultOperator is ClientSideTransformOperator)
            {
                _serverSide = false;
            }
            else
            {
                if (!_serverSide)
                {
                    throw new NotSupportedException("Processing server-side result operator after doing client-side ones.  We've got the ordering wrong...");
                }
            }

            if (resultOperator is FirstResultOperator)
            {
                ProcessFirstOperator((FirstResultOperator) resultOperator);
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
            else if (resultOperator is SingleResultOperator)
            {
                ProcessSingleOperator((SingleResultOperator) resultOperator);
            }
            else if (resultOperator is ContainsResultOperator)
            {
                ProcessContainsOperator((ContainsResultOperator) resultOperator);
            }
            else if (resultOperator is NonAggregatingGroupBy)
            {
                ProcessNonAggregatingGroupBy((NonAggregatingGroupBy)resultOperator, queryModel);
            }
            else if (resultOperator is ClientSideSelect)
            {
                ProcessClientSideSelect((ClientSideSelect)resultOperator);
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

        private void ProcessContainsOperator(ContainsResultOperator resultOperator)
        {
            var itemExpression =
                HqlGeneratorExpressionTreeVisitor.Visit(resultOperator.Item, _parameters, _requiredHqlParameters)
                                                 .AsExpression();

            var from = GetFromRangeClause();
            var source = from.Children.First();

            if (source is HqlParameter)
            {
                // This is an "in" style statement
                _treeNode = _hqlTreeBuilder.In(itemExpression, source);

            }
            else
            {
                // This is an "exists" style statement
                AddWhereClause(_hqlTreeBuilder.Equality(
                                   _hqlTreeBuilder.Ident(GetFromAlias().AstNode.Text),
                                   itemExpression));

                _treeNode = _hqlTreeBuilder.Exists((HqlQuery)_treeNode);
            }
        }

        private HqlAlias GetFromAlias()
        {
            return _treeNode.NodesPreOrder.Single(n => n is HqlRange).Children.Single(n => n is HqlAlias) as HqlAlias;
        }

        private HqlRange GetFromRangeClause()
        {
            return _treeNode.NodesPreOrder.Single(n => n is HqlRange).As<HqlRange>();
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
                _additionalCriteria.Add((q, p) => q.SetMaxResults((int) p[parameterName.Name].First));
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
                _additionalCriteria.Add((q, p) => q.SetFirstResult((int)p[parameterName.Name].First));
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
		}

        private void GroupBy<TSource, TKey, TResult>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TResult>> elementSelector)
        {
            IQueryable<object> list = null;

            var x = list.Cast<TSource>().GroupBy(keySelector, elementSelector);
        }

    	private static System.Type SourceOf(Expression keySelector)
    	{
    		return new GroupByKeySourceFinder().Visit(keySelector).Type;
    	}

    	private void ProcessGroupByOperator(GroupResultOperator resultOperator)
		{
            AddGroupByClause(_hqlTreeBuilder.GroupBy(HqlGeneratorExpressionTreeVisitor.Visit(resultOperator.KeySelector, _parameters, _requiredHqlParameters).AsExpression()));
		}

		private void ProcessFirstOperator(FirstResultOperator resultOperator)
		{
		    var firstMethod = resultOperator.ReturnDefaultWhenEmpty
		                          ? ReflectionHelper.GetMethod(() => Queryable.FirstOrDefault<object>(null))
		                          : ReflectionHelper.GetMethod(() => Queryable.First<object>(null));

		    ProcessFirstOrSingle(firstMethod);
		}

        private void ProcessSingleOperator(SingleResultOperator resultOperator)
        {
            var firstMethod = resultOperator.ReturnDefaultWhenEmpty
                                  ? ReflectionHelper.GetMethod(() => Queryable.SingleOrDefault<object>(null))
                                  : ReflectionHelper.GetMethod(() => Queryable.Single<object>(null));

            ProcessFirstOrSingle(firstMethod);
        }

        private void ProcessFirstOrSingle(MethodInfo target)
        {
            target = target.MakeGenericMethod(_currentEvaluationType.DataType);

            var parameter = Expression.Parameter(_previousEvaluationType.DataType, null);

            var lambda = Expression.Lambda(
                            Expression.Call(
                                target,
                                parameter),
                            parameter);

            _additionalCriteria.Add((q, p) => q.SetMaxResults(1));
            _postExecuteTransformers.Add(lambda);
        }

		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
		{
		    _currentEvaluationType = selectClause.GetOutputDataInfo();

            var visitor = new SelectClauseVisitor(typeof(object[]), _parameters, _requiredHqlParameters);

            visitor.Visit(selectClause.Selector);

            if (visitor.ProjectionExpression != null)
            {
                _itemTransformers.Add(visitor.ProjectionExpression);
            }

            AddSelectClause(_hqlTreeBuilder.Select(visitor.GetHqlNodes()));

			base.VisitSelectClause(selectClause, queryModel);
		}

		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			// Visit the predicate to build the query
            AddWhereClause(HqlGeneratorExpressionTreeVisitor.Visit(whereClause.Predicate, _parameters, _requiredHqlParameters).AsBooleanExpression());
		}

		public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
		{
			foreach (Ordering clause in orderByClause.Orderings)
			{
                AddOrderByClause(HqlGeneratorExpressionTreeVisitor.Visit(clause.Expression, _parameters, _requiredHqlParameters).AsExpression(),
                                clause.OrderingDirection == OrderingDirection.Asc
		                        	? _hqlTreeBuilder.Ascending()
		                        	: (HqlDirectionStatement) _hqlTreeBuilder.Descending());
			}
		}

		public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
		{
			var equalityVisitor = new EqualityHqlGenerator(_parameters, _requiredHqlParameters);
			var whereClause = equalityVisitor.Visit(joinClause.InnerKeySelector, joinClause.OuterKeySelector);

            AddWhereClause(whereClause);

            AddFromClause(_hqlTreeBuilder.Range(HqlGeneratorExpressionTreeVisitor.Visit(joinClause.InnerSequence, _parameters, _requiredHqlParameters),
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
				    AddFromClause(_hqlTreeBuilder.Join(
                                         HqlGeneratorExpressionTreeVisitor.Visit(fromClause.FromExpression, _parameters, _requiredHqlParameters).AsExpression(),
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
				AddFromClause(_hqlTreeBuilder.Range(
                                     HqlGeneratorExpressionTreeVisitor.Visit(fromClause.FromExpression, _parameters, _requiredHqlParameters),
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