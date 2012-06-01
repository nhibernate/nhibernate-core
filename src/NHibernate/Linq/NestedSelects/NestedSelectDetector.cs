using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.NestedSelects
{
	class NestedSelectDetector : NhExpressionTreeVisitor
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