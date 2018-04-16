using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Expressions
{
	/// <summary>
	///     Represents an expression that has been nominated for direct inclusion in the SELECT clause.
	///     This bypasses the standard nomination process and assumes that the expression can be converted
	///     directly to SQL.
	/// </summary>
	/// <remarks>
	///     Used in the nomination of GroupBy key expressions to ensure that matching select clauses
	///     are generated the same way.
	/// </remarks>
	public class NhNominatedExpression : NhExpression
	{
		public NhNominatedExpression(Expression expression)
		{
			Expression = expression;
		}

		public override System.Type Type => Expression.Type;

		public Expression Expression { get; }

		protected override Expression Accept(NhExpressionVisitor visitor)
		{
			return visitor.VisitNhNominated(this);
		}

		protected override Expression VisitChildren(ExpressionVisitor visitor)
		{
			var newExpression = visitor.Visit(Expression);

			return newExpression != Expression
				? new NhNominatedExpression(newExpression)
				: this;
		}
	}
}
