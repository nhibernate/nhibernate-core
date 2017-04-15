using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class NhDistinctExpression : NhAggregatedExpression
	{
		public NhDistinctExpression(Expression expression)
			: base(expression) { }

		public override NhExpressionType NhNodeType => NhExpressionType.Distinct;

		public override Expression CreateNew(Expression expression) => new NhDistinctExpression(expression);
	}
}