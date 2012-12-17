using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.ReWriters
{
	/// <summary>
	/// This re-writer is responsible to re-write a query without a body (no where-clause and so on).
	/// </summary>
	public class QueryReferenceExpressionFlattener : NhExpressionTreeVisitor
	{
		private readonly QueryModel _model;
		// NOTE: Skip/Take are not completelly flattenable since Take(10).Skip(5).Take(2) should result in a subqueries-tsunami (so far not common understanding from our users)
		private static readonly List<System.Type> FlattenableResultOperactors = new List<System.Type>
			{
				typeof (CacheableResultOperator),
				typeof (TimeoutResultOperator),
				typeof (SkipResultOperator),
				typeof (TakeResultOperator),
			};

		private QueryReferenceExpressionFlattener(QueryModel model)
		{
			_model = model;
		}

		public static void ReWrite(QueryModel model)
		{
			var visitor = new QueryReferenceExpressionFlattener(model);
			model.TransformExpressions(visitor.VisitExpression);
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression subQuery)
		{
			var hasBodyClauses = subQuery.QueryModel.BodyClauses.Count > 0;
			if(hasBodyClauses)
			{
				return base.VisitSubQueryExpression(subQuery);
			}
			var resultOperators = subQuery.QueryModel.ResultOperators;
			if (resultOperators.Count == 0 || HasJustAllFlattenableOperator(resultOperators))
			{
				var selectQuerySource = subQuery.QueryModel.SelectClause.Selector as QuerySourceReferenceExpression;

				if (selectQuerySource != null && selectQuerySource.ReferencedQuerySource == subQuery.QueryModel.MainFromClause)
				{
					foreach (var resultOperator in resultOperators)
					{
						_model.ResultOperators.Add(resultOperator);
					}

					return subQuery.QueryModel.MainFromClause.FromExpression;
				}
			}

			return base.VisitSubQueryExpression(subQuery);
		}

		private bool HasJustAllFlattenableOperator(IEnumerable<ResultOperatorBase> resultOperators)
		{
			return resultOperators.All(x => FlattenableResultOperactors.Contains(x.GetType()));
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

			return base.VisitQuerySourceReferenceExpression(expression);
		}
	}
}