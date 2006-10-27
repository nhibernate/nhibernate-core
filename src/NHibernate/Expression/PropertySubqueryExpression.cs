using System;

namespace NHibernate.Expression
{
	/// <summary>
	/// A comparison between a property value in the outer query and the
	///  result of a subquery
	/// </summary>
	public class PropertySubqueryExpression : SubqueryExpression
	{
		private String propertyName;

		internal PropertySubqueryExpression(String propertyName, String op, String quantifier, DetachedCriteria dc)
			: base(op, quantifier, dc)
		{
			this.propertyName = propertyName;
		}

		protected override String ToLeftSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return criteriaQuery.GetColumn(criteria, propertyName);
		}
	}
}