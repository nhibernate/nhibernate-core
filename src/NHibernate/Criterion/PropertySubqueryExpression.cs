using System;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{
	/// <summary>
	/// A comparison between a property value in the outer query and the
	///  result of a subquery
	/// </summary>
	[Serializable]
	public class PropertySubqueryExpression : SubqueryExpression
	{
		private String propertyName;

		internal PropertySubqueryExpression(String propertyName, String op, String quantifier, DetachedCriteria dc)
			: base(op, quantifier, dc)
		{
			this.propertyName = propertyName;
		}

		protected override SqlString ToLeftSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new SqlString(criteriaQuery.GetColumn(criteria, propertyName));
		}
	}
}