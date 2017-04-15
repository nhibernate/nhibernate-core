using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Expressions
{
	public class NhStarExpression : NhSimpleExpression
	{
		public NhStarExpression(Expression expression)
			: base(expression) { }

		public override NhExpressionType NhNodeType => NhExpressionType.Star;

		public override Expression CreateNew(Expression expression) => new NhStarExpression(expression);

		protected override Expression Accept(NhExpressionVisitor visitor)
			=> visitor.VisitNhStar(this);
	}
}