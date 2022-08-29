using System.Linq.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.GroupBy
{
	internal class KeySelectorVisitor : RelinqExpressionVisitor
	{
		private readonly GroupResultOperator _groupBy;

		public KeySelectorVisitor(GroupResultOperator groupBy)
		{
			_groupBy = groupBy;
		}

		protected override Expression VisitMember(MemberExpression expression)
		{
			if (expression.IsGroupingKeyOf(_groupBy))
			{
				return _groupBy.KeySelector;
			}
			return base.VisitMember(expression);
		}
	}
}
