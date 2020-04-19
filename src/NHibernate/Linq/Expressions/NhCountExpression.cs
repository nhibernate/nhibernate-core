using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Expressions
{
	public abstract class NhCountExpression : NhAggregatedExpression
	{
		protected NhCountExpression(Expression expression, System.Type type)
			: base(expression, type)
		{
		}

		public override bool AllowsNullableReturnType => false;

		protected override Expression Accept(NhExpressionVisitor visitor)
		{
			return visitor.VisitNhCount(this);
		}
	}

	public class NhShortCountExpression : NhCountExpression
	{
		public NhShortCountExpression(Expression expression)
			: base(expression, typeof(int))
		{
		}

		public override Expression CreateNew(Expression expression)
		{
			return new NhShortCountExpression(expression);
		}
	}

	public class NhLongCountExpression : NhCountExpression
	{
		public NhLongCountExpression(Expression expression)
			: base(expression, typeof(long))
		{
		}

		public override Expression CreateNew(Expression expression)
		{
			return new NhLongCountExpression(expression);
		}
	}
}
