using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public abstract class NhCountExpression : NhAggregatedExpression
	{
		protected NhCountExpression(Expression expression, System.Type type)
			: base(expression, type, NhExpressionType.Count) {}

		public abstract NhCountExpression CreateNew(Expression expression);
	}

	public class NhShortCountExpression : NhCountExpression
	{
		public NhShortCountExpression(Expression expression)
			: base(expression, typeof (int)) {}

		public override NhCountExpression CreateNew(Expression expression)
		{
			return new NhShortCountExpression(expression);
		}
	}

	public class NhLongCountExpression : NhCountExpression
	{
		public NhLongCountExpression(Expression expression)
			: base(expression, typeof (long)) {}

		public override NhCountExpression CreateNew(Expression expression)
		{
			return new NhLongCountExpression(expression);
		}
	}
}