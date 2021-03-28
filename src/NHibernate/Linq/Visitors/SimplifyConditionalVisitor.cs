using System.Linq.Expressions;
using NHibernate.Util;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Some conditional expressions can be reduced to just their IfTrue or IfFalse part.
	/// </summary>
	internal class SimplifyConditionalVisitor :RelinqExpressionVisitor
	{
		protected override Expression VisitConditional(ConditionalExpression expression)
		{
			var testExpression = Visit(expression.Test);

			bool testExprResult;
			if (VisitorUtil.IsBooleanConstant(testExpression, out testExprResult))
			{
				if (testExprResult)
					return Visit(expression.IfTrue);

				return Visit(expression.IfFalse);
			}

			return base.VisitConditional(expression);
		}

		protected override Expression VisitBinary(BinaryExpression expression)
		{
			// See NH-3423. Conditional expression where the test expression is a comparison
			// of a construction expression and null will happen in WCF DS.

			if (IsConstructionToNullComparison(expression))
			{
				// The result of a construction operation is always non-null. So if it's being compared to
				// a null constant, we can simplify it to a boolean constant.
				if (expression.NodeType == ExpressionType.Equal)
					return Expression.Constant(false);

				if (expression.NodeType == ExpressionType.NotEqual)
					return Expression.Constant(true);
			}

			return base.VisitBinary(expression);
		}

		private static bool IsConstruction(Expression expression)
		{
			return expression is NewExpression || expression is MemberInitExpression;
		}

		private static bool IsConstructionToNullComparison(Expression expression)
		{
			var testExpression = expression as BinaryExpression;

			if (testExpression != null)
			{
				if ((IsConstruction(testExpression.Left) && VisitorUtil.IsNullConstant(testExpression.Right))
				    || (IsConstruction(testExpression.Right) && VisitorUtil.IsNullConstant(testExpression.Left)))
				{
					return true;
				}
			}

			return false;
		}
	}
}
