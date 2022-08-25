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
	public class QueryReferenceExpressionFlattener : RelinqExpressionVisitor
	{
		private readonly QueryModel _model;

		internal static readonly System.Type[] FlattenableResultOperators =
		{
			typeof(LockResultOperator),
			typeof(FetchLazyPropertiesResultOperator),
			typeof(FetchOneRequest),
			typeof(FetchManyRequest)
		};

		private QueryReferenceExpressionFlattener(QueryModel model)
		{
			_model = model;
		}

		public static void ReWrite(QueryModel model)
		{
			var visitor = new QueryReferenceExpressionFlattener(model);
			model.TransformExpressions(visitor.Visit);
		}

		protected override Expression VisitSubQuery(SubQueryExpression subQuery)
		{
			var subQueryModel = subQuery.QueryModel;
			var hasBodyClauses = subQueryModel.BodyClauses.Count > 0;
			if (hasBodyClauses)
			{
				return base.VisitSubQuery(subQuery);
			}

			var resultOperators = subQueryModel.ResultOperators;
			if (resultOperators.Count == 0 || HasJustAllFlattenableOperator(resultOperators))
			{
				if (subQueryModel.SelectClause.Selector is QuerySourceReferenceExpression selectQuerySource && selectQuerySource.ReferencedQuerySource == subQueryModel.MainFromClause)
				{
					foreach (var resultOperator in resultOperators)
					{
						_model.ResultOperators.Add(resultOperator);
					}

					return subQueryModel.MainFromClause.FromExpression;
				}
			}

			return base.VisitSubQuery(subQuery);
		}

		private static bool HasJustAllFlattenableOperator(IEnumerable<ResultOperatorBase> resultOperators)
		{
			return resultOperators.All(x => FlattenableResultOperators.Contains(x.GetType()));
		}

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
		{

			if (expression.ReferencedQuerySource is FromClauseBase fromClauseBase &&
				fromClauseBase.FromExpression is QuerySourceReferenceExpression &&
				expression.Type == fromClauseBase.FromExpression.Type)
			{
				return fromClauseBase.FromExpression;
			}

			return base.VisitQuerySourceReference(expression);
		}
	}
}
