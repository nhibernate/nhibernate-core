using System;
using System.Linq;
using NHibernate.Util;
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
				queryModel.ResultOperators.OfType<CastResultOperator>().ToArray().ForEach(castOperator => queryModel.ResultOperators.Remove(castOperator));
			}
			if (resultOperator is AnyResultOperator)
			{
				queryModel.ResultOperators.OfType<FetchOneRequest>().ToArray().ForEach(op => queryModel.ResultOperators.Remove(op));
				queryModel.ResultOperators.OfType<FetchManyRequest>().ToArray().ForEach(op => queryModel.ResultOperators.Remove(op));
			}
			base.VisitResultOperator(resultOperator, queryModel, index);
		}
	}
}
