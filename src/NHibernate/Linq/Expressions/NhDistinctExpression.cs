using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class NhDistinctExpression : NhAggregatedExpression
	{
		public NhDistinctExpression(Expression expression)
			: base(expression, NhExpressionType.Distinct)
		{
		}
	}
}