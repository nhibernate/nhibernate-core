using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class NhCountExpression : NhAggregatedExpression
	{
		public NhCountExpression(Expression expression)
			: base(expression, typeof(int), NhExpressionType.Count)
		{
		}
	}
}