using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class NhMaxExpression : NhAggregatedExpression
	{
		public NhMaxExpression(Expression expression)
			: base(expression) { }

		public override NhExpressionType NhNodeType => NhExpressionType.Max;

		public override Expression CreateNew(Expression expression) => new NhMaxExpression(expression);
	}
}