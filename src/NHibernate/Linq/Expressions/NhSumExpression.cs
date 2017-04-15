using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class NhSumExpression : NhAggregatedExpression
	{
		public NhSumExpression(Expression expression)
			: base(expression) { }

		public override NhExpressionType NhNodeType => NhExpressionType.Sum;

		public override Expression CreateNew(Expression expression) => new NhSumExpression(expression);
	}
}