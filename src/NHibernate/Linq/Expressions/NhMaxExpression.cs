using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Expressions
{
	public class NhMaxExpression : NhAggregatedExpression
	{
		public NhMaxExpression(Expression expression)
			: base(expression)
		{
		}

		public override Expression CreateNew(Expression expression)
		{
			return new NhMaxExpression(expression);
		}

		protected override Expression Accept(NhExpressionVisitor visitor)
		{
			return visitor.VisitNhMax(this);
		}
	}
}
