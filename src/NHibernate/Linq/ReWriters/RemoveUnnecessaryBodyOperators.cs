using System;
using System.Linq;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.EagerFetching;

namespace NHibernate.Linq.ReWriters
{
	public class RemoveUnnecessaryBodyOperators : QueryModelVisitorBase
	{
		private RemoveUnnecessaryBodyOperators() {}

		public static void ReWrite(QueryModel queryModel)
		{
			var rewriter = new RemoveUnnecessaryBodyOperators();

			rewriter.VisitQueryModel(queryModel);
		}

		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
			if (resultOperator is CountResultOperator || resultOperator is LongCountResultOperator)
			{
				// For count operators, we can remove any order-by result operators
				var bodyClauses = queryModel.BodyClauses.OfType<OrderByClause>().ToList();
				foreach (var orderby in bodyClauses)
				{
					queryModel.BodyClauses.Remove(orderby);
				}
			}
			if (resultOperator is CastResultOperator)
			{
				Array.ForEach(queryModel.ResultOperators.OfType<CastResultOperator>().ToArray(), castOperator=> queryModel.ResultOperators.Remove(castOperator));
			}
			if (resultOperator is AnyResultOperator)
			{
				Array.ForEach(queryModel.ResultOperators.OfType<FetchOneRequest>().ToArray(), op => queryModel.ResultOperators.Remove(op));
				Array.ForEach(queryModel.ResultOperators.OfType<FetchManyRequest>().ToArray(), op => queryModel.ResultOperators.Remove(op));
			}
			base.VisitResultOperator(resultOperator, queryModel, index);
		}
	}
}