using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class NhAverageExpression : NhAggregatedExpression
	{
		public NhAverageExpression(Expression expression) : base(expression, NhExpressionType.Average)
		{
		}
	}
}