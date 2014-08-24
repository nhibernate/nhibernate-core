using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class NhMinExpression : NhAggregatedExpression
	{
		public NhMinExpression(Expression expression)
			: base(expression, NhExpressionType.Min)
		{
		}

		public override Expression CreateNew(Expression expression)
		{
			return new NhMinExpression(expression);
		}
	}
}