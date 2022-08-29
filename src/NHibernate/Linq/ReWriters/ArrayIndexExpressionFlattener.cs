using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.ReWriters
{
	public class ArrayIndexExpressionFlattener : RelinqExpressionVisitor
	{
		public static void ReWrite(QueryModel model)
		{
			var visitor = new ArrayIndexExpressionFlattener();
			model.TransformExpressions(visitor.Visit);
		}

		protected override Expression VisitBinary(BinaryExpression expression)
		{
			var visitedExpression = base.VisitBinary(expression);

			if (visitedExpression.NodeType != ExpressionType.ArrayIndex)
				return visitedExpression;

			var index = expression.Right as ConstantExpression;
			if (index == null)
				return visitedExpression;

			var expressionList = expression.Left as NewArrayExpression;
			if (expressionList == null || expressionList.NodeType != ExpressionType.NewArrayInit)
				return visitedExpression;

			return Visit(expressionList.Expressions[(int) index.Value]);
		}

		protected override Expression VisitSubQuery(SubQueryExpression expression)
		{
			ReWrite(expression.QueryModel);
			return expression; // Note that we modifiy the (mutable) QueryModel, we return an unchanged expression
		}
	}
}
