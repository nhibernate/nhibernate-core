using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public abstract class NhCountExpression : NhAggregatedExpression
	{
		protected NhCountExpression(Expression expression, System.Type type)
			: base(expression, type) { }

		public override NhExpressionType NhNodeType => NhExpressionType.Count;

		public bool IsCountStar => Expression is NhStarExpression;
	}

	public class NhShortCountExpression : NhCountExpression
	{
		public NhShortCountExpression(Expression expression)
			: base(expression, typeof(int)) { }

		public override Expression CreateNew(Expression expression) => new NhShortCountExpression(expression);
	}

	public class NhLongCountExpression : NhCountExpression
	{
		public NhLongCountExpression(Expression expression)
			: base(expression, typeof(long)) { }

		public override Expression CreateNew(Expression expression) => new NhLongCountExpression(expression);
	}
}