using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Expressions
{
	public class NhSumExpression : NhAggregatedExpression
	{
		public NhSumExpression(Expression expression)
			: base(expression)
		{
		}

		public override Expression CreateNew(Expression expression)
		{
			return new NhSumExpression(expression);
		}

		protected override Expression Accept(NhExpressionVisitor visitor)
		{
			return visitor.VisitNhSum(this);
		}
	}
}
