using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class NhSumExpression : NhAggregatedExpression
	{
		public NhSumExpression(Expression expression)
			: base(expression, NhExpressionType.Sum)
		{
		}

		public override Expression CreateNew(Expression expression)
		{
			return new NhSumExpression(expression);
		}
	}
}