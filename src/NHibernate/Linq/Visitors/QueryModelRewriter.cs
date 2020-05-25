using System.Linq.Expressions;
using NHibernate.Linq.GroupBy;
using NHibernate.Linq.GroupJoin;
using NHibernate.Linq.NestedSelects;
using NHibernate.Linq.ReWriters;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public static class QueryModelRewriter
	{
		/// <summary>
		/// Rewrites the given <see cref="QueryModel"/> and all <see cref="QueryModel"/> that are found inside it.
		/// </summary>
		/// <param name="rootQueryModel">The query model to rewrite.</param>
		/// <param name="parameters">The visitor parameters.</param>
		public static void Rewrite(QueryModel rootQueryModel, VisitorParameters parameters)
		{
			Rewrite(rootQueryModel, parameters, true);
			// Rewrite sub-queries
			var rewriter = new SubqueryRewriterVisitor(parameters);
			rootQueryModel.TransformExpressions(rewriter.Visit);
		}

		internal static void Rewrite(QueryModel queryModel, VisitorParameters parameters, bool root)
		{
			// Expand conditionals in subquery FROM clauses into multiple subqueries
			if (root)
			{
				// This expander works recursively
				SubQueryConditionalExpander.ReWrite(queryModel);
			}

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

			// Expand coalesced and conditional joins to their logical equivalents
			ConditionalQueryReferenceExpander.ReWrite(queryModel);

			// Move OrderBy clauses to end
			MoveOrderByToEndRewriter.ReWrite(queryModel);

			// Give a rewriter provided by the session factory a chance to
			// rewrite the query.
			var rewriterFactory = parameters.SessionFactory.Settings.QueryModelRewriterFactory;
			var customVisitor = rewriterFactory?.CreateVisitor(parameters);
			customVisitor?.VisitQueryModel(queryModel);

			// rewrite any operators that should be applied on the outer query
			// by flattening out the sub-queries that they are located in
			parameters.AddQueryModelRewriterResult(queryModel, ResultOperatorRewriter.Rewrite(queryModel));

			// Remove conditional expressions where they can be reduced to just their IfTrue or IfFalse part.
			SimplifyConditionalRewriter.Rewrite(queryModel);

			// Identify and name query sources
			QuerySourceIdentifier.Visit(parameters.QuerySourceNamer, queryModel);
		}

		private class SubqueryRewriterVisitor : RelinqExpressionVisitor
		{
			private readonly VisitorParameters _parameters;

			public SubqueryRewriterVisitor(VisitorParameters parameters)
			{
				_parameters = parameters;
			}

			public override Expression Visit(Expression node)
			{
				if (node?.NodeType == ExpressionType.Extension &&
					node is SubQueryExpression subQueryExpression &&
					!_parameters.QueryModelRewriterResults.ContainsKey(subQueryExpression.QueryModel))
				{
					Rewrite(subQueryExpression.QueryModel, _parameters, false);
					subQueryExpression.QueryModel.TransformExpressions(Visit);
				}

				return base.Visit(node);
			}
		}
	}
}
