using System;
using System.Linq;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.EagerFetching;

namespace NHibernate.Linq.ReWriters
{
	public class RemoveUnnecessaryBodyOperators : NhQueryModelVisitorBase
	{
		private RemoveUnnecessaryBodyOperators() {}

		public static void ReWrite(QueryModel queryModel)
		{
			var rewriter = new RemoveUnnecessaryBodyOperators();

			rewriter.VisitQueryModel(queryModel);
		}

		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
			if (resultOperator is CountResultOperator || 
			    resultOperator is LongCountResultOperator || 
			    resultOperator is ContainsResultOperator ||
			    resultOperator is AnyResultOperator ||
			    resultOperator is AllResultOperator)
			{
				// For these operators, we can remove any order-by clause
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
				ResultOperatorRemover.Remove(queryModel, x => x is FetchRequestBase);
			}
			base.VisitResultOperator(resultOperator, queryModel, index);
		}
	}
}
