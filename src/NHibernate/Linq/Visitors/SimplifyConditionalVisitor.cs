using System.Linq.Expressions;
using NHibernate.Util;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Some conditional expressions can be redured to just their IfTrue or IfFalse part.
	/// </summary>
	internal class SimplifyConditionalVisitor :ExpressionTreeVisitor
	{
		protected override Expression VisitConditionalExpression(ConditionalExpression expression)
		{
			var testExpression = VisitExpression(expression.Test);

			bool testExprResult;
			if (VisitorUtil.IsBooleanConstant(testExpression, out testExprResult))
			{
				if (testExprResult)
					return VisitExpression(expression.IfTrue);

				return VisitExpression(expression.IfFalse);
			}

			return base.VisitConditionalExpression(expression);
		}


		protected override Expression VisitBinaryExpression(BinaryExpression expression)
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

			return base.VisitBinaryExpression(expression);
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