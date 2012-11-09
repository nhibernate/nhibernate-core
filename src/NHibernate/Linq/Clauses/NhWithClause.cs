namespace NHibernate.Linq.Clauses
{
	using System.Linq.Expressions;
	using Remotion.Linq.Clauses;
	using Remotion.Linq.Clauses.ExpressionTreeVisitors;

	public class NhWithClause : WhereClause
	{
		public NhWithClause(Expression predicate)
			: base(predicate)
		{
		}

		public override string ToString()
		{
			return "with " + FormattingExpressionTreeVisitor.Format(Predicate);
		}
	}
}