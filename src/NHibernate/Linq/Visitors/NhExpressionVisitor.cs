using System;
using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public class NhExpressionVisitor : RelinqExpressionVisitor
	{
		protected internal virtual Expression VisitNhStar(NhStarExpression expression)
		{
			return VisitExtension(expression);
		}

		protected internal virtual Expression VisitNhNew(NhNewExpression expression)
		{
			return VisitExtension(expression);
		}

		protected internal virtual Expression VisitNhAggregated(NhAggregatedExpression node)
		{
			return VisitExtension(node);
		}

		protected internal virtual Expression VisitNhDistinct(NhDistinctExpression expression)
		{
			return VisitNhAggregated(expression);
		}

		protected internal virtual Expression VisitNhCount(NhCountExpression expression)
		{
			return VisitNhAggregated(expression);
		}

		protected internal virtual Expression VisitNhSum(NhSumExpression expression)
		{
			return VisitNhAggregated(expression);
		}

		protected internal virtual Expression VisitNhMax(NhMaxExpression expression)
		{
			return VisitNhAggregated(expression);
		}

		protected internal virtual Expression VisitNhMin(NhMinExpression expression)
		{
			return VisitNhAggregated(expression);
		}

		protected internal virtual Expression VisitNhAverage(NhAverageExpression expression)
		{
			return VisitNhAggregated(expression);
		}

		protected internal virtual Expression VisitNhNominated(NhNominatedExpression expression)
		{
			return VisitExtension(expression);
		}
	}
}
