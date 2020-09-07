using System;
using System.Linq;
using NHibernate.Linq.Expressions;
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

		internal static void RemoveUnnecessaryOrderByClauses(QueryModel queryModel)
		{
			if (IsOrderByNeeded(queryModel))
				return;

			// For these operators, we can remove any order-by clause
			var bodyClauses = queryModel.BodyClauses;
			for (int i = bodyClauses.Count - 1; i >= 0; i--)
			{
				if (bodyClauses[i] is OrderByClause)
					bodyClauses.RemoveAt(i);
			}
		}

		internal static bool IsOrderByNeeded(QueryModel queryModel)
		{
			switch (queryModel.ResultOperators.Count)
			{
				case 1:
					var r = queryModel.ResultOperators[0];
					return !(r is AnyResultOperator || r is AllResultOperator || r is ContainsResultOperator);  
				case 0:
					return !(queryModel.SelectClause.Selector is NhAggregatedExpression);
			}

			return true;
		}

		public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
		{
			if (resultOperator is CountResultOperator || resultOperator is LongCountResultOperator)
			{
				// For count operators, we can remove any order-by clause
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
