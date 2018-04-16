using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Expressions
{
	public class NhMinExpression : NhAggregatedExpression
	{
		public NhMinExpression(Expression expression)
			: base(expression)
		{
		}

		public override Expression CreateNew(Expression expression)
		{
			return new NhMinExpression(expression);
		}

		protected override Expression Accept(NhExpressionVisitor visitor)
		{
			return visitor.VisitNhMin(this);
		}
	}
}
