using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using NHibernate.Linq.GroupBy;
using NHibernate.Linq.GroupJoin;
using NHibernate.Linq.ResultOperators;
using NHibernate.Linq.ReWriters;
using NHibernate.Linq.Visitors.ResultOperatorProcessors;
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

            // Add left joins for references
		    AddLeftJoinsReWriter.ReWrite(queryModel, parameters.SessionFactory);

			var visitor = new QueryModelVisitor(parameters, root, queryModel);
			visitor.Visit();

			return visitor.GetTranslation();
		}

        private static readonly ResultOperatorMap ResultOperatorMap;

        private readonly List<Action<IQuery, IDictionary<string, Tuple<object, IType>>>> _additionalCriteria = new List<Action<IQuery, IDictionary<string, Tuple<object, IType>>>>();
        private readonly List<LambdaExpression> _listTransformers = new List<LambdaExpression>();
        private readonly List<LambdaExpression> _itemTransformers = new List<LambdaExpression>();
        private readonly List<LambdaExpression> _postExecuteTransformers = new List<LambdaExpression>();
        private readonly bool _root;
        private bool _serverSide = true;

        public HqlTreeNode Root { get; private set; }
        public VisitorParameters VisitorParameters { get; private set; }
        public IStreamedDataInfo CurrentEvaluationType { get; private set; }
        public IStreamedDataInfo PreviousEvaluationType { get; private set; }
        public HqlTreeBuilder TreeBuilder { get; private set; }
        public QueryModel Model { get; private set; }

        static QueryModelVisitor()
        {
            ResultOperatorMap = new ResultOperatorMap();

            ResultOperatorMap.Add<AggregateResultOperator, ProcessAggregate>();
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
        }

        private QueryModelVisitor(VisitorParameters visitorParameters, bool root, QueryModel queryModel)
		{
            VisitorParameters = visitorParameters;
            Model = queryModel;
            _root = root;
            TreeBuilder = new HqlTreeBuilder();
            Root = TreeBuilder.Query(TreeBuilder.SelectFrom(TreeBuilder.From()));
		}

        private void Visit()
        {
            VisitQueryModel(Model);
        }


        private ExpressionToHqlTranslationResults GetTranslation()
		{
            if (_root)
            {
                DetectOuterExists();
            }

            return new ExpressionToHqlTranslationResults(Root,
                                                         _itemTransformers,
                                                         _listTransformers,
                                                         _postExecuteTransformers,
                                                         _additionalCriteria);
		}

	    private void DetectOuterExists()
	    {
            if (Root is HqlExists)
            {
                Root = Root.Children.First();

                _additionalCriteria.Add((q, p) => q.SetMaxResults(1));

                Expression<Func<IEnumerable<object>, bool>> x = l => l.Any();

                _listTransformers.Add(x);
            }
	    }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
		{
            AddFromClause(TreeBuilder.Range(
                             HqlGeneratorExpressionTreeVisitor.Visit(fromClause.FromExpression, VisitorParameters),
                             TreeBuilder.Alias(fromClause.ItemName)));

			base.VisitMainFromClause(fromClause, queryModel);
		}


        private void AddWhereClause(HqlBooleanExpression where)
        {
            var currentWhere = Root.NodesPreOrder.Where(n => n is HqlWhere).FirstOrDefault();

            if (currentWhere == null)
            {
                currentWhere = TreeBuilder.Where(where);
                Root.As<HqlQuery>().AddChild(currentWhere);
            }
            else
            {
                var currentClause = (HqlBooleanExpression)currentWhere.Children.Single();

                currentWhere.ClearChildren();
                currentWhere.AddChild(TreeBuilder.BooleanAnd(currentClause, where));
            }
        }

        private void AddFromClause(HqlTreeNode from)
        {
            Root.NodesPreOrder.Where(n => n is HqlFrom).First().AddChild(from);
        }

        private void AddSelectClause(HqlTreeNode select)
        {
            Root.NodesPreOrder.Where(n => n is HqlSelectFrom).First().AddChild(select);
        }

        private void AddGroupByClause(HqlGroupBy groupBy)
        {
            Root.As<HqlQuery>().AddChild(groupBy);
        }

        private void AddOrderByClause(HqlExpression orderBy, HqlDirectionStatement direction)
        {
            var orderByRoot = Root.NodesPreOrder.Where(n => n is HqlOrderBy).FirstOrDefault();

            if (orderByRoot == null)
            {
                orderByRoot = TreeBuilder.OrderBy();
                Root.As<HqlQuery>().AddChild(orderByRoot);
            }
            
            orderByRoot.AddChild(orderBy);
            orderByRoot.AddChild(direction);
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

            var results = ResultOperatorMap.Process(resultOperator, this);

            if (results.AdditionalCriteria != null)
            {
                _additionalCriteria.Add(results.AdditionalCriteria);
            }
            if (results.GroupBy != null)
            {
                AddGroupByClause(results.GroupBy);
            }
            if (results.ListTransformer != null)
            {
                _listTransformers.Add(results.ListTransformer);
            }
            if (results.PostExecuteTransformer != null)
            {
                _postExecuteTransformers.Add(results.PostExecuteTransformer);
            }
            if (results.WhereClause != null)
            {
                AddWhereClause(results.WhereClause);
            }
            if (results.TreeNode != null)
            {
                Root = results.TreeNode;
            }
        }

        private void GroupBy<TSource, TKey, TResult>(Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TResult>> elementSelector)
        {
            IQueryable<object> list = null;

            var x = list.Cast<TSource>().GroupBy(keySelector, elementSelector);
        }

		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
		{
		    CurrentEvaluationType = selectClause.GetOutputDataInfo();

            var visitor = new SelectClauseVisitor(typeof(object[]), VisitorParameters);

            visitor.Visit(selectClause.Selector);

            if (visitor.ProjectionExpression != null)
            {
                _itemTransformers.Add(visitor.ProjectionExpression);
            }

            AddSelectClause(TreeBuilder.Select(visitor.GetHqlNodes()));

			base.VisitSelectClause(selectClause, queryModel);
		}

		public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
		{
			// Visit the predicate to build the query
            AddWhereClause(HqlGeneratorExpressionTreeVisitor.Visit(whereClause.Predicate, VisitorParameters).AsBooleanExpression());
		}

		public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
		{
			foreach (Ordering clause in orderByClause.Orderings)
			{
                AddOrderByClause(HqlGeneratorExpressionTreeVisitor.Visit(clause.Expression, VisitorParameters).AsExpression(),
                                clause.OrderingDirection == OrderingDirection.Asc
		                        	? TreeBuilder.Ascending()
		                        	: (HqlDirectionStatement) TreeBuilder.Descending());
			}
		}

		public override void VisitJoinClause(JoinClause joinClause, QueryModel queryModel, int index)
		{
			var equalityVisitor = new EqualityHqlGenerator(VisitorParameters);
			var whereClause = equalityVisitor.Visit(joinClause.InnerKeySelector, joinClause.OuterKeySelector);

            AddWhereClause(whereClause);

            AddFromClause(TreeBuilder.Range(HqlGeneratorExpressionTreeVisitor.Visit(joinClause.InnerSequence, VisitorParameters),
                                                   TreeBuilder.Alias(joinClause.ItemName)));
		}

		public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
		{
            if (fromClause is LeftJoinClause)
            {
                // It's a left join
                AddFromClause(TreeBuilder.LeftJoin(
                                     HqlGeneratorExpressionTreeVisitor.Visit(fromClause.FromExpression, VisitorParameters).AsExpression(),
                                     TreeBuilder.Alias(fromClause.ItemName)));
            }
			else if (fromClause.FromExpression is MemberExpression)
			{
				var member = (MemberExpression) fromClause.FromExpression;

				if (member.Expression is QuerySourceReferenceExpression)
				{
					// It's a join
				    AddFromClause(TreeBuilder.Join(
                                         HqlGeneratorExpressionTreeVisitor.Visit(fromClause.FromExpression, VisitorParameters).AsExpression(),
				                         TreeBuilder.Alias(fromClause.ItemName)));
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
				AddFromClause(TreeBuilder.Range(
                                     HqlGeneratorExpressionTreeVisitor.Visit(fromClause.FromExpression, VisitorParameters),
									 TreeBuilder.Alias(fromClause.ItemName)));

			}

			base.VisitAdditionalFromClause(fromClause, queryModel, index);
		}

		public override void VisitGroupJoinClause(GroupJoinClause groupJoinClause, QueryModel queryModel, int index)
		{
            throw new NotImplementedException();
		}
	}
}