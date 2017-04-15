using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Expressions
{
	public abstract class NhExpression : Expression
	{
		public override ExpressionType NodeType => ExpressionType.Extension;

		public abstract NhExpressionType NhNodeType { get; }

		protected override Expression Accept(ExpressionVisitor visitor)
		{
			if (visitor is NhExpressionVisitor nhVisitor)
				Accept(nhVisitor);
			return base.Accept(visitor);
		}

		protected abstract Expression Accept(NhExpressionVisitor visitor);
	}

	public abstract class NhSimpleExpression : NhExpression
	{
		protected NhSimpleExpression(Expression expression)
			: this(expression, expression.Type) { }

		protected NhSimpleExpression(Expression expression, System.Type expressionType)
		{
			Expression = expression;
			Type = expressionType;
		}

		public Expression Expression { get; }

		public override System.Type Type { get; }

		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			var newExpression = visitor.Visit(Expression);

			return newExpression != Expression
				? CreateNew(newExpression)
				: this;
		}

		public abstract Expression CreateNew(Expression expression);
	}
}