using System.Linq.Expressions;

namespace NHibernate.Linq.Expressions
{
	public class OrderExpression : NHExpression
	{
		public OrderExpression(Expression source, LambdaExpression selector, OrderType orderType, System.Type type)
			: base(NHExpressionType.Order, type)
		{
			Source = source;
			Selector = selector;
			OrderType = orderType;
		}

		public Expression Source { get; protected set; }
		public LambdaExpression Selector { get; protected set; }
		public OrderType OrderType { get; protected set; }

		public override string ToString()
		{
			return string.Format("({0}).OrderBy({1} {2})", Source, Selector, OrderType);
		}
	}
}