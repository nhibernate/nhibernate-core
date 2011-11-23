using System;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using NHibernate.Linq.GroupBy;
using NHibernate.Linq.GroupJoin;
using NHibernate.Linq.ResultOperators;
using NHibernate.Linq.ReWriters;
using NHibernate.Linq.Visitors.ResultOperatorProcessors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.EagerFetching;

namespace NHibernate.Linq.Visitors
{
	public class QueryModelVisitor : QueryModelVisitorBase
	{
		public static ExpressionToHqlTranslationResults GenerateHqlQuery(QueryModel queryModel, VisitorParameters parameters, bool root)
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

			// Add joins for references
			AddJoinsReWriter.ReWrite(queryModel, parameters.SessionFactory);

			// Move OrderBy clauses to end
			MoveOrderByToEndRewriter.ReWrite(queryModel);

			// rewrite any operators that should be applied on the outer query
			// by flattening out the sub-queries that they are located in
			ResultOperatorRewriterResult result = ResultOperatorRewriter.Rewrite(queryModel);

			// Identify and name query sources
			QuerySourceIdentifier.Visit(parameters.QuerySourceNamer, queryModel);

			var visitor = new QueryModelVisitor(parameters, root, queryModel) { RewrittenOperatorResult = result };
			visitor.Visit();

			return visitor._hqlTree.GetTranslation();
		}

		private readonly IntermediateHqlTree _hqlTree;
		private static readonly ResultOperatorMap ResultOperatorMap;
		private bool _serverSide = true;

		public VisitorParameters VisitorParameters { get; private set; }

		public IStreamedDataInfo CurrentEvaluationType { get; private set; }

		public IStreamedDataInfo PreviousEvaluationType { get; private set; }

		public QueryModel Model { get; private set; }

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
			ResultOperatorMap.Add<AnyResultOperator, ProcessAny>();
			ResultOperatorMap.Add<AllResultOperator, ProcessAll>();
			ResultOperatorMap.Add<FetchOneRequest, ProcessFetchOne>();
			ResultOperatorMap.Add<FetchManyRequest, ProcessFetchMany>();
			ResultOperatorMap.Add<CacheableResultOperator, ProcessCacheable>();
			ResultOperatorMap.Add<OfTypeResultOperator, ProcessOfType>();
			ResultOperatorMap.Add<CastResultOperator, ProcessCast>();
		}

		private QueryModelVisitor(VisitorParameters visitorParameters, bool root, QueryModel queryModel)
		{
			VisitorParameters = visitorParameters;
			Model = queryModel;
			_hqlTree = new IntermediateHqlTree(root);
		}

		private void Visit()
		{
			VisitQueryModel(Model);
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

			if (fromClause is NhJoinClause)
			{
				if (((NhJoinClause)fromClause).IsInner)
				{
					_hqlTree.AddFromClause(
						_hqlTree.TreeBuilder.Join(
							HqlGeneratorExpressionTreeVisitor.Visit(fromClause.FromExpression, VisitorParameters).AsExpression(),
							_hqlTree.TreeBuilder.Alias(querySourceName)));
				}
				else
				{
					_hqlTree.AddFromClause(
						_hqlTree.TreeBuilder.LeftJoin(
							HqlGeneratorExpressionTreeVisitor.Visit(fromClause.FromExpression, VisitorParameters).AsExpression(),
							_hqlTree.TreeBuilder.Alias(querySourceName)));
				}
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
			// Visit the predicate to build the query
			_hqlTree.AddWhereClause(HqlGeneratorExpressionTreeVisitor.Visit(whereClause.Predicate, VisitorParameters).AsBooleanExpression());
		}

		public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
		{
			foreach (Ordering clause in orderByClause.Orderings)
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