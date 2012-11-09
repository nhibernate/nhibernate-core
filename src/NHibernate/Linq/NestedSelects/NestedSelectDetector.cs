using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.NestedSelects
{
	class NestedSelectDetector : ExpressionTreeVisitor
	{
		public bool HasSubquery { get; set; }
		public SubQueryExpression Expression { get; private set; }

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			HasSubquery |= expression.QueryModel.ResultOperators.Count == 0;
			Expression = expression;
			return base.VisitSubQueryExpression(expression);
		}
	}
}