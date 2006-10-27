using System;

namespace NHibernate.Expression
{
	public class ExistsSubqueryExpression : SubqueryExpression
	{
		protected override String ToLeftSqlString(ICriteria criteria, ICriteriaQuery outerQuery)
		{
			return "";
		}

		internal ExistsSubqueryExpression(String quantifier, DetachedCriteria dc) : base(null, quantifier, dc)
		{
		}
	}
}