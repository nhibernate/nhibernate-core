using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using System.Linq.Expressions;

namespace NHibernate.Linq.Clauses
{
	public class NhHavingClause : WhereClause
	{
		public NhHavingClause(Expression predicate) 
			: base(predicate)
		{
		}

		public override string ToString()
		{
			return "having " + FormattingExpressionTreeVisitor.Format(Predicate);
		}
	}
}