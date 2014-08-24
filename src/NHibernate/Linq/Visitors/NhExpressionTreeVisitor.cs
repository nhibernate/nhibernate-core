using System;
using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	public class NhExpressionTreeVisitor : ExpressionTreeVisitor
	{
		public override Expression VisitExpression(Expression expression)
		{
			if (expression == null)
			{
				return null;
			}

			switch ((NhExpressionType)expression.NodeType)
			{
				case NhExpressionType.Average:
				case NhExpressionType.Min:
				case NhExpressionType.Max:
				case NhExpressionType.Sum:
				case NhExpressionType.Count:
				case NhExpressionType.Distinct:
					return VisitNhAggregate((NhAggregatedExpression)expression);
				case NhExpressionType.New:
					return VisitNhNew((NhNewExpression)expression);
				case NhExpressionType.Star:
					return VisitNhStar((NhStarExpression)expression);
			}

			// Keep this variable for easy examination during debug.
			var expr = base.VisitExpression(expression);
			return expr;
		}

		protected virtual Expression VisitNhStar(NhStarExpression expression)
		{
			return expression.Accept(this);
		}

		protected virtual Expression VisitNhNew(NhNewExpression expression)
		{
			return expression.Accept(this);
		}

		protected virtual Expression VisitNhAggregate(NhAggregatedExpression expression)
		{
			switch ((NhExpressionType)expression.NodeType)
			{
				case NhExpressionType.Average:
					return VisitNhAverage((NhAverageExpression)expression);
				case NhExpressionType.Min:
					return VisitNhMin((NhMinExpression)expression);
				case NhExpressionType.Max:
					return VisitNhMax((NhMaxExpression)expression);
				case NhExpressionType.Sum:
					return VisitNhSum((NhSumExpression)expression);
				case NhExpressionType.Count:
					return VisitNhCount((NhCountExpression)expression);
				case NhExpressionType.Distinct:
					return VisitNhDistinct((NhDistinctExpression)expression);
				default:
					throw new ArgumentException();
			}
		}

		protected virtual Expression VisitNhDistinct(NhDistinctExpression expression)
		{
			return expression.Accept(this);
		}

		protected virtual Expression VisitNhCount(NhCountExpression expression)
		{
			return expression.Accept(this);
		}

		protected virtual Expression VisitNhSum(NhSumExpression expression)
		{
			return expression.Accept(this);
		}

		protected virtual Expression VisitNhMax(NhMaxExpression expression)
		{
			return expression.Accept(this);
		}

		protected virtual Expression VisitNhMin(NhMinExpression expression)
		{
			return expression.Accept(this);
		}

		protected virtual Expression VisitNhAverage(NhAverageExpression expression)
		{
			return expression.Accept(this);
		}
	}
}