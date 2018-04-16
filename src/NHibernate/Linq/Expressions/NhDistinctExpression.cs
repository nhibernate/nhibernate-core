using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Expressions
{
	public class NhDistinctExpression : NhAggregatedExpression
	{
		public NhDistinctExpression(Expression expression)
			: base(expression)
		{
		}

		public override Expression CreateNew(Expression expression)
		{
			return new NhDistinctExpression(expression);
		}

		protected override Expression Accept(NhExpressionVisitor visitor)
		{
			return visitor.VisitNhDistinct(this);
		}
	}
}
