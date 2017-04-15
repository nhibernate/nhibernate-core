using System;
using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public class NhExpressionVisitor : RelinqExpressionVisitor
	{
		public override Expression Visit(Expression expression)
		{
			if (expression == null)
			{
				return null;
			}

			// Keep this variable for easy examination during debug.
			var expr = base.Visit(expression);
			return expr;
		}

		public virtual Expression VisitNhStar(NhStarExpression expression)
			=> VisitExtension(expression);

		public virtual Expression VisitNhNew(NhNewExpression expression)
			=> VisitExtension(expression);

		internal virtual Expression VisitNhNominated(NhNominatedExpression expression)
			=> VisitExtension(expression);

		public virtual Expression VisitNhAggregate(NhAggregatedExpression expression)
		{
			switch (expression.NhNodeType)
			{
				case NhExpressionType.Average:
					return VisitNhAverage(expression);
				case NhExpressionType.Min:
					return VisitNhMin(expression);
				case NhExpressionType.Max:
					return VisitNhMax(expression);
				case NhExpressionType.Sum:
					return VisitNhSum(expression);
				case NhExpressionType.Count:
					return VisitNhCount(expression);
				case NhExpressionType.Distinct:
					return VisitNhDistinct(expression);
				default:
					throw new ArgumentException($"Unsupported NH node type {expression.NhNodeType}.", nameof(expression));
			}
		}

		protected virtual Expression VisitNhDistinct(NhSimpleExpression expression)
			=> VisitExtension(expression);

		protected virtual Expression VisitNhCount(NhSimpleExpression expression)
			=> VisitExtension(expression);

		protected virtual Expression VisitNhSum(NhSimpleExpression expression)
			=> VisitExtension(expression);

		protected virtual Expression VisitNhMax(NhSimpleExpression expression)
			=> VisitExtension(expression);

		protected virtual Expression VisitNhMin(NhSimpleExpression expression)
			=> VisitExtension(expression);

		protected virtual Expression VisitNhAverage(NhSimpleExpression expression)
			=> VisitExtension(expression);
	}
}