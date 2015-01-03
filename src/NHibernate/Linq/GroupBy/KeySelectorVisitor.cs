using System.Linq.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.GroupBy
{
	internal class KeySelectorVisitor : ExpressionTreeVisitor
	{
		private readonly GroupResultOperator _groupBy;

		public KeySelectorVisitor(GroupResultOperator groupBy)
		{
			_groupBy = groupBy;
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			if (expression.IsGroupingKeyOf(_groupBy))
			{
				return _groupBy.KeySelector;
			}
			return base.VisitMemberExpression(expression);
		}
	}
}