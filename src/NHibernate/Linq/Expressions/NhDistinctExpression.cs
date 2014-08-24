using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class NhDistinctExpression : NhAggregatedExpression
	{
		public NhDistinctExpression(Expression expression)
			: base(expression, NhExpressionType.Distinct)
		{
		}

		public override Expression CreateNew(Expression expression)
		{
			return new NhDistinctExpression(expression);
		}
	}
}