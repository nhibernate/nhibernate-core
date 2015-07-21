using System;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{
	/// <summary>
	/// A comparison between a property value in the outer query and the
	///  result of a subquery
	/// </summary>
	[Serializable]
	public class SelectSubqueryExpression : SubqueryExpression
	{
		internal SelectSubqueryExpression(DetachedCriteria dc)
			: base(null, null, dc)
		{
		}

		protected override SqlString ToLeftSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return SqlString.Empty;
		}
	}
}
