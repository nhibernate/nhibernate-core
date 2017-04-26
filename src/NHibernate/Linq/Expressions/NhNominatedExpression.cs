using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.Expressions
{
	/// <summary>
	/// Represents an expression that has been nominated for direct inclusion in the SELECT clause.
	/// This bypasses the standard nomination process and assumes that the expression can be converted 
	/// directly to SQL.
	/// </summary>
	/// <remarks>
	/// Used in the nomination of GroupBy key expressions to ensure that matching select clauses
	/// are generated the same way.
	/// </remarks>
	internal class NhNominatedExpression : ExtensionExpression
	{
		public Expression Expression { get; private set; }

		public NhNominatedExpression(Expression expression) : base(expression.Type, (ExpressionType)NhExpressionType.Nominator)
		{
			Expression = expression;
		}

		protected override Expression VisitChildren(ExpressionTreeVisitor visitor)
		{
			var newExpression = visitor.VisitExpression(Expression);

			return newExpression != Expression
				? new NhNominatedExpression(newExpression)
				: this;
		}
	}
}