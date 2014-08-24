using System.Linq.Expressions;
using Remotion.Linq.Parsing;
using Remotion.Linq.Parsing.ExpressionTreeVisitors;

namespace NHibernate.Linq.Visitors
{
	internal class NhPartialEvaluatingExpressionTreeVisitor : ExpressionTreeVisitor
	{
		protected override Expression VisitConstantExpression(ConstantExpression expression)
		{
			var value = expression.Value as Expression;
			if (value == null)
			{
				return base.VisitConstantExpression(expression);
			}

			return EvaluateIndependentSubtrees(value);
		}

		public static Expression EvaluateIndependentSubtrees(Expression expression)
		{
			var evaluatedExpression = PartialEvaluatingExpressionTreeVisitor.EvaluateIndependentSubtrees(expression);
			return new NhPartialEvaluatingExpressionTreeVisitor().VisitExpression(evaluatedExpression);
		}
	}
}