using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Clauses;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.GroupBy;
using NHibernate.Linq.GroupJoin;
using NHibernate.Linq.NestedSelects;
using NHibernate.Linq.ResultOperators;
using NHibernate.Linq.ReWriters;
using NHibernate.Linq.Visitors.ResultOperatorProcessors;
using NHibernate.Util;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.EagerFetching;

namespace NHibernate.Linq.Visitors
{
	public class QueryModelVisitor : QueryModelVisitorBase
	{
		public static ExpressionToHqlTranslationResults GenerateHqlQuery(QueryModel queryModel, VisitorParameters parameters, bool root,
			NhLinqExpressionReturnType? rootReturnType)
		{
			NestedSelectRewriter.ReWrite(queryModel, parameters.SessionFactory);

			// Remove unnecessary body operators
			RemoveUnnecessaryBodyOperators.ReWrite(queryModel);

			// Merge aggregating result operators (distinct, count, sum etc) into the select clause
			MergeAggregatingResultsRewriter.ReWrite(queryModel);

			// Swap out non-aggregating group-bys
			NonAggregatingGroupByRewriter.ReWrite(queryModel);

			// Rewrite aggregate group-by statements
			AggregatingGroupByRewriter.ReWrite(queryModel);

			// Rewrite aggregating group-joins
			AggregatingGroupJoinRewriter.ReWrite(queryModel);

			// Rewrite non-aggregating group-joins
			NonAggregatingGroupJoinRewriter.ReWrite(queryModel);

			SubQueryFromClauseFlattener.ReWrite(queryModel);

			// Rewrite left-joins
			LeftJoinRewriter.ReWrite(queryModel);

			// Rewrite paging
			PagingRewriter.ReWrite(queryModel);

			// Flatten pointless subqueries
			QueryReferenceExpressionFlattener.ReWrite(queryModel);

			// Flatten array index access to query references
			ArrayIndexExpressionFlattener.ReWrite(queryModel);

			// Add joins for references
			AddJoinsReWriter.ReWrite(queryModel, parameters);

			// Move OrderBy clauses to end
			MoveOrderByToEndRewriter.ReWrite(queryModel);

			// Give a rewriter provided by the session factory a chance to
			// rewrite the query.
			var rewriterFactory = parameters.SessionFactory.Settings.QueryModelRewriterFactory;
			if (rewriterFactory != null)
			{
				var customVisitor = rewriterFactory.CreateVisitor(parameters);
				if (customVisitor != null)
					customVisitor.VisitQueryModel(queryModel);
			}

			// rewrite any operators that should be applied on the outer query
			// by flattening out the sub-queries that they are located in
			var result = ResultOperatorRewriter.Rewrite(queryModel);

			// Identify and name query sources
			QuerySourceIdentifier.Visit(parameters.QuerySourceNamer, queryModel);

			var visitor = new QueryModelVisitor(parameters, root, queryModel, rootReturnType)
			{
				RewrittenOperatorResult = result,
			};
			visitor.Visit();

			return visitor._hqlTree.GetTranslation();
		}

		private readonly IntermediateHqlTree _hqlTree;
		private readonly NhLinqExpressionReturnType? _rootReturnType;
		private static readonly ResultOperatorMap ResultOperatorMap;
		private bool _serverSide = true;

		public VisitorParameters VisitorParameters { get; }

		public IStreamedDataInfo CurrentEvaluationType { get; private set; }

		public IStreamedDataInfo PreviousEvaluationType { get; private set; }

		public QueryModel Model { get; }

		public ResultOperatorRewriterResult RewrittenOperatorResult { get; private set; }

		static QueryModelVisitor()
		{
			// TODO - reflection to build map
			ResultOperatorMap = new ResultOperatorMap();

			ResultOperatorMap.Add<AggregateResultOperator, ProcessAggregate>();
			ResultOperatorMap.Add<AggregateFromSeedResultOperator, ProcessAggregateFromSeed>();
			ResultOperatorMap.Add<FirstResultOperator, ProcessFirst>();
			ResultOperatorMap.Add<TakeResultOperator, ProcessTake>();
			ResultOperatorMap.Add<SkipResultOperator, ProcessSkip>();
			ResultOperatorMap.Add<GroupResultOperator, ProcessGroupBy>();
			ResultOperatorMap.Add<SingleResultOperator, ProcessSingle>();
			ResultOperatorMap.Add<ContainsResultOperator, ProcessContains>();
			ResultOperatorMap.Add<NonAggregatingGroupBy, ProcessNonAggregatingGroupBy>();
			ResultOperatorMap.Add<ClientSideSelect, ProcessClientSideSelect>();
			ResultOperatorMap.Add<ClientSideSelect2, ProcessClientSideSelect2>();
			ResultOperatorMap.Add<AnyResultOperator, ProcessAny>();
			ResultOperatorMap.Add<AllResultOperator, ProcessAll>();
			ResultOperatorMap.Add<FetchOneRequest, ProcessFetchOne>();
			ResultOperatorMap.Add<FetchManyRequest, ProcessFetchMany>();
			ResultOperatorMap.Add<CacheableResultOperator, ProcessCacheable>();
			ResultOperatorMap.Add<TimeoutResultOperator, ProcessTimeout>();
			ResultOperatorMap.Add<OfTypeResultOperator, ProcessOfType>();
			ResultOperatorMap.Add<CastResultOperator, ProcessCast>();
		}

		private QueryModelVisitor(VisitorParameters visitorParameters, bool root, QueryModel queryModel,
			NhLinqExpressionReturnType? rootReturnType)
		{
			VisitorParameters = visitorParameters;
			Model = queryModel;
			_rootReturnType = root ? rootReturnType : null;
			_hqlTree = new IntermediateHqlTree(root);
		}

		private void Visit()
		{
			VisitQueryModel(Model);
			AddAdditionalPostExecuteTransformer();
		}

		private void AddAdditionalPostExecuteTransformer()
		{
			if (_rootReturnType == NhLinqExpressionReturnType.Scalar && Model.ResultTypeOverride != null)
			{
				// NH-3850: handle polymorphic scalar results aggregation
				switch ((NhExpressionType)Model.SelectClause.Selector.NodeType)
				{
					case NhExpressionType.Average:
						// Polymorphic case complex to handle and not implemented. (HQL query must be reshaped for adding
						// additional data to allow a meaningful overall average computation.)
						// Leaving it untouched for allowing non polymorphic cases to work.
						break;
					case NhExpressionType.Count:
						AddPostExecuteTransformerForCount();
						break;
					case NhExpressionType.Max:
						AddPostExecuteTransformerForResultAggregate(ReflectionCache.EnumerableMethods.MaxDefinition);
						break;
					case NhExpressionType.Min:
						AddPostExecuteTransformerForResultAggregate(ReflectionCache.EnumerableMethods.MinDefinition);
						break;
					case NhExpressionType.Sum:
						AddPostExecuteTransformerForSum();
						break;
				}
			}
		}

		private void AddPostExecuteTransformerForCount()
		{
			// Count results have to be summed. No null case to take into account.
			var elementType = Model.ResultTypeOverride;
			var inputListType = typeof(IEnumerable<>).MakeGenericType(elementType);
			var inputList = Expression.Parameter(inputListType, "inputList");
			// Sum has no suitable generic overload, throw in Sum on int, then the code using it
			// will check and adjust it if it is long instead of int (GetAggregateMethodCall does that).
			var aggregateCall = GetAggregateMethodCall(ReflectionCache.EnumerableMethods.SumOnInt, inputListType, elementType, inputList);
			_hqlTree.AddPostExecuteTransformer(Expression.Lambda(aggregateCall, inputList));
		}

		private void AddPostExecuteTransformerForResultAggregate(MethodInfo aggregateMethodTemplate)
		{
			var elementType = Model.ResultTypeOverride;

			LambdaExpression aggregateLambda;

			// string may be aggregated and are not Nullable<> but can be null, and qualifies as reference
			if (elementType.IsNullableOrReference())
			{
				var inputListType = typeof(IEnumerable<>).MakeGenericType(elementType);
				var inputList = Expression.Parameter(inputListType, "inputList");
				var aggregateCall = GetAggregateMethodCall(aggregateMethodTemplate, inputListType, elementType, inputList);
				aggregateLambda = Expression.Lambda(aggregateCall, inputList);
			}
			else
			{
				// On non nullable, we have to preserve the behavior "nothing to aggregate => exception",
				// while supporting the polymorphic case "nothing to aggregate for some classes but something to aggregate
				// for others" => aggregate of others.
				// For this, overriding result type of concrete queries as nullable, aggregating concrete results,
				// casting back to non nullable (and failing if null).
				// This still causes a change: the expected failure case will yield an InvalidOperationException instead of
				// a GenericADOException with an ArgumentNullException inner exception.

				var nullableElementType = typeof(Nullable<>).MakeGenericType(elementType);
				var nullableInputListType = typeof(IEnumerable<>).MakeGenericType(nullableElementType);
				var nullableInputList = Expression.Parameter(nullableInputListType, "nullableInputList");
				_hqlTree.ExecuteResultTypeOverride = nullableElementType;

				var aggregateCall = GetAggregateMethodCall(aggregateMethodTemplate, nullableInputListType, nullableElementType, nullableInputList);

				var convert = Expression.Convert(aggregateCall, elementType);
				aggregateLambda = Expression.Lambda(convert, nullableInputList);
			}

			_hqlTree.AddPostExecuteTransformer(aggregateLambda);
		}

		private void AddPostExecuteTransformerForSum()
		{
			// Sum is a bit hard, due to an additional mismatch between linq-to-objects and sql semantics on sum, when there are nothing to sum.
			// linq-to-objets => 0, sql => null
			// We have to emulate the sql behavior when aggregating polymorphic results.
			var elementType = Model.ResultTypeOverride;
			var concreteQueryElementType = elementType;
			var elementTypeIsNullable = elementType.IsNullable();
			if (!elementTypeIsNullable)
			{
				// Same as in AddPostExecuteTransformerForResultAggregate, override the result type of concrete queries to nullable.
				// But then, let the nullable sql emulation yield null if there is nothing to sum, and finally try casting the result
				// to non nullable.
				concreteQueryElementType = typeof(Nullable<>).MakeGenericType(elementType);
				_hqlTree.ExecuteResultTypeOverride = concreteQueryElementType;
			}

			var inputListType = typeof(IEnumerable<>).MakeGenericType(concreteQueryElementType);
			var inputList = Expression.Parameter(inputListType, "inputList");
			// Sum has no suitable generic overload, throw in Sum on int, then the code using it
			// will check and adjust it if it is long instead of int (GetAggregateMethodCall does that).
			var aggregateCall = GetAggregateMethodCall(ReflectionCache.EnumerableMethods.SumOnInt, inputListType, concreteQueryElementType, inputList);

			var allMethod = ReflectionCache.EnumerableMethods.AllDefinition.MakeGenericMethod(concreteQueryElementType);
			var element = Expression.Parameter(concreteQueryElementType, "element");
			// The concreteQueryElementType is always nullable, see above if block.
			var noSumCondition = Expression.Equal(element, Expression.Constant(null, concreteQueryElementType));
			var allCall = Expression.Call(allMethod, inputList, Expression.Lambda(noSumCondition, element));

			Expression conditionalAggregateCall = Expression.Condition(allCall,
				Expression.Constant(null, concreteQueryElementType),
				aggregateCall);
			if (!elementTypeIsNullable)
				conditionalAggregateCall = Expression.Convert(conditionalAggregateCall, elementType);
			_hqlTree.AddPostExecuteTransformer(Expression.Lambda(conditionalAggregateCall, inputList));
		}

		private MethodCallExpression GetAggregateMethodCall(MethodInfo aggregateMethodTemplate, System.Type inputListType,
			System.Type elementType, Expression inputList)
		{
			MethodInfo aggregateMethod;
			if (aggregateMethodTemplate.IsGenericMethodDefinition)
			{
				aggregateMethod = aggregateMethodTemplate.MakeGenericMethod(elementType);
			}
			else
			{
				// Ensure we use the right overload.
				aggregateMethod = ReflectHelper.GetMethodOverload(aggregateMethodTemplate, inputListType);
			}
			return Expression.Call(aggregateMethod, inputList);
		}

		public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
		{
			var querySourceName = VisitorParameters.QuerySourceNamer.GetName(fromClause);
			var hqlExpressionTree = HqlGeneratorExpressionTreeVisitor.Visit(fromClause.FromExpression, VisitorParameters);

			_hqlTree.AddFromClause(_hqlTree.TreeBuilder.Range(hqlExpressionTree, _hqlTree.TreeBuilder.Alias(querySourceName)));

			// apply any result operators that were rewritten
			if (RewrittenOperatorResult != null)
			{
				CurrentEvaluationType = RewrittenOperatorResult.EvaluationType;
				foreach (ResultOperatorBase rewrittenOperator in RewrittenOperatorResult.RewrittenOperators)
				{
					VisitResultOperator(rewrittenOperator, queryModel, -1);
				}
			}

			base.VisitMainFromClause(fromClause, queryModel);
		}

		public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
		{
			var querySourceName = VisitorParameters.QuerySourceNamer.GetName(fromClause);

			var joinClause = fromClause as NhJoinClause;
			if (joinClause != null)
			{
				VisitNhJoinClause(querySourceName, joinClause);
			}
			else if (fromClause.FromExpression is MemberExpression)
			{
				// It's a join
				_hqlTree.AddFromClause(
					_hqlTree.TreeBuilder.Join(
						HqlGeneratorExpressionTreeVisitor.Visit(fromClause.FromExpression, VisitorParameters).AsExpression(),
						_hqlTree.TreeBuilder.Alias(querySourceName)));
			}
			else
			{
				// TODO - exact same code as in MainFromClause; refactor this out
				_hqlTree.AddFromClause(
					_hqlTree.TreeBuilder.Range(
						HqlGeneratorExpressionTreeVisitor.Visit(fromClause.FromExpression, VisitorParameters),
						_hqlTree.TreeBuilder.Alias(querySourceName)));

			}

			base.VisitAdditionalFromClause(fromClause, queryModel, index);
		}

		private void VisitNhJoinClause(string querySourceName, NhJoinClause joinClause)
		{
			var expression = HqlGeneratorExpressionTreeVisitor.Visit(joinClause.FromExpression, VisitorParameters).AsExpression();
			var alias = _hqlTree.TreeBuilder.Alias(querySourceName);

			HqlTreeNode hqlJoin;
			if (joinClause.IsInner)
			{
				hqlJoin = _hqlTree.TreeBuilder.Join(expression, @alias);
			}
			else
			{
				hqlJoin = _hqlTree.TreeBuilder.LeftJoin(expression, @alias);
			}

			foreach (var withClause in joinClause.Restrictions)
			{
				var booleanExpression = HqlGeneratorExpressionTreeVisitor.Visit(withClause.Predicate, VisitorParameters).ToBooleanExpression();
				hqlJoin.AddChild(_hqlTree.TreeBuilder.With(booleanExpression));
			}

			_hqlTree.AddFromClause(hqlJoin);
		}

		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
			PreviousEvaluationType = CurrentEvaluationType;
			CurrentEvaluationType = resultOperator.GetOutputDataInfo(PreviousEvaluationType);

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

			ResultOperatorMap.Process(resultOperator, this, _hqlTree);
		}

		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
		{
			CurrentEvaluationType = selectClause.GetOutputDataInfo();

			var visitor = new SelectClauseVisitor(typeof(object[]), VisitorParameters);

			visitor.Visit(selectClause.Selector);

			if (visitor.ProjectionExpression != null)
			{
				_hqlTree.AddItemTransformer(visitor.ProjectionExpression);
			}

			_hqlTree.AddSelectClause(_hqlTree.TreeBuilder.Select(visitor.GetHqlNodes()));

			base.VisitSelectClause(selectClause, queryModel);
		}

		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			var visitor = new SimplifyConditionalVisitor();
			whereClause.Predicate = visitor.VisitExpression(whereClause.Predicate);

			// Visit the predicate to build the query
			var expression = HqlGeneratorExpressionTreeVisitor.Visit(whereClause.Predicate, VisitorParameters).ToBooleanExpression();
			if (whereClause is NhHavingClause)
			{
				_hqlTree.AddHavingClause(expression);
			}
			else
			{
				_hqlTree.AddWhereClause(expression);
			}
		}

		public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
		{
			foreach (var clause in orderByClause.Orderings)
			{
				_hqlTree.AddOrderByClause(HqlGeneratorExpressionTreeVisitor.Visit(clause.Expression, VisitorParameters).AsExpression(),
								clause.OrderingDirection == OrderingDirection.Asc
									? _hqlTree.TreeBuilder.Ascending()
									: (HqlDirectionStatement)_hqlTree.TreeBuilder.Descending());
			}
		}

		public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
		{
			var equalityVisitor = new EqualityHqlGenerator(VisitorParameters);
			var whereClause = equalityVisitor.Visit(joinClause.InnerKeySelector, joinClause.OuterKeySelector);

			_hqlTree.AddWhereClause(whereClause);

			_hqlTree.AddFromClause(
				_hqlTree.TreeBuilder.Range(
					HqlGeneratorExpressionTreeVisitor.Visit(joinClause.InnerSequence, VisitorParameters),
					_hqlTree.TreeBuilder.Alias(joinClause.ItemName)));
		}

		public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
		{
			throw new NotImplementedException();
		}
	}
}