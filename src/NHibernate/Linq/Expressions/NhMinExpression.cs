using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class NhMinExpression : NhAggregatedExpression
	{
		public NhMinExpression(Expression expression)
			: base(expression) { }

		public override NhExpressionType NhNodeType => NhExpressionType.Min;

		public override Expression CreateNew(Expression expression) => new NhMinExpression(expression);
	}
}