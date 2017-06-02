using System.Linq.Expressions;
using NHibernate.Linq.Visitors;

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
	internal class NhNominatedExpression : NhSimpleExpression
	{
		public NhNominatedExpression(Expression expression)
			: base(expression) { }

		public override NhExpressionType NhNodeType => NhExpressionType.Nominator;

		public override Expression CreateNew(Expression expression) => new NhNominatedExpression(expression);

		protected override Expression Accept(NhExpressionVisitor visitor)
			=> visitor.VisitNhNominated(this);
	}
}