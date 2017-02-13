using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.ReWriters
{
	public class QueryReferenceExpressionFlattener : ExpressionTreeVisitor
	{
		private readonly QueryModel _model;

		internal static readonly System.Type[] FlattenableResultOperators =
		{
			typeof (CacheableResultOperator),
			typeof (TimeoutResultOperator),
			typeof (FetchOneRequest),
			typeof (FetchManyRequest)
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
			var subQueryModel = subQuery.QueryModel;
			var hasBodyClauses = subQueryModel.BodyClauses.Count > 0;
			if (hasBodyClauses)
			{
				return base.VisitSubQueryExpression(subQuery);
			}

			var resultOperators = subQueryModel.ResultOperators;
			if (resultOperators.Count == 0 || HasJustAllFlattenableOperator(resultOperators))
			{
				var selectQuerySource = subQueryModel.SelectClause.Selector as QuerySourceReferenceExpression;

				if (selectQuerySource != null && selectQuerySource.ReferencedQuerySource == subQueryModel.MainFromClause)
				{
					foreach (var resultOperator in resultOperators)
					{
						_model.ResultOperators.Add(resultOperator);
					}

					return subQueryModel.MainFromClause.FromExpression;
				}
			}

			return base.VisitSubQueryExpression(subQuery);
		}

		private static bool HasJustAllFlattenableOperator(IEnumerable<ResultOperatorBase> resultOperators)
		{
			return resultOperators.All(x => FlattenableResultOperators.Contains(x.GetType()));
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